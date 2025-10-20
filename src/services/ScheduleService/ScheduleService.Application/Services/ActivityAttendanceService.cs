using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Services
{
    public class ActivityAttendanceService : IActivityAttendanceService
    {
        private readonly IScheduleActivityRepository _scheduleActivityRepository;
        private readonly IScheduleParticipantRepository _participantRepository;
        private readonly IActivityAttendanceRepository _attendanceRepository;
        private readonly IScheduleMediaService _mediaService;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly INotificationService _notificationService;
        private readonly IAuthServiceClient _authServiceClient;

        public ActivityAttendanceService(IScheduleActivityRepository scheduleActivityRepository, IScheduleParticipantRepository participantRepository
            , IActivityAttendanceRepository activityAttendanceRepository, IScheduleMediaService mediaService
            , IRealtimeNotifier realtimeNotifier, INotificationService notificationService, IAuthServiceClient authServiceClient)
        {
            _scheduleActivityRepository = scheduleActivityRepository;
            _participantRepository = participantRepository;
            _attendanceRepository = activityAttendanceRepository;
            _mediaService = mediaService;
            _realtimeNotifier = realtimeNotifier;
            _notificationService = notificationService;
            _authServiceClient = authServiceClient;
        }

        private DateTime ConvertToUtc7(DateTime localDateTime)
        {
            // Convert sang giờ VN (UTC+7)
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(localDateTime, vnTimeZone);
            return localTime;
        }

        public async Task<ActivityAttendance> CheckInAsync(Guid userId, AttendanceRequest request)
        {
            // Validate activity
            var activity = await _scheduleActivityRepository.GetActivityByIdAsync(request.ActivityId);
            if (activity == null)
            {
                throw new Exception("No activity was found");
            }

            // Validate participant
            var participant = await _participantRepository.GetByUserIdAndScheduleIdAsync(userId, activity.ScheduleId);
            if (participant == null)
            {
                throw new Exception("No participant was found");
            }

            // Validate participant in activity
            var participants = await _participantRepository.GetAllParticipantByScheduleIdAsync(activity.ScheduleId);
            if (participants == null)
            {
                throw new Exception("No participant is in the activity");
            }

            if (participants.FirstOrDefault(p => p.Id == participant.Id) == null)
            {
                throw new Exception("The participant is not in this activity");
            }

            var existedAttendance = await _attendanceRepository.GetCheckInAttendanceByActivityAndParticipantAsync(activity.Id, participant.Id);
            if (existedAttendance != null)
            {
                throw new Exception("An activity just check-in 1 time");
            }


            var attendance = new ActivityAttendance
            {
                Id = Guid.NewGuid(),
                CheckInTime = DateTime.UtcNow,
                CheckOutTime = null,
                Status = AttendanceStatus.CheckIn,
                ActivityId = request.ActivityId,
                ParticipantId = participant.Id
            };

            await _attendanceRepository.CreateAttendanceAsync(attendance);

            // Create media
            var mediaRequest = new UploadScheduleMediaRequest
            {
                File = request.File,
                Description = request.Description,
                UploadMethod = MediaMethod.CheckIn,
                ActivityId = activity.Id,
                ScheduleId = null
            };
            await _mediaService.UploadAsync(mediaRequest);

            var user = await _authServiceClient.GetCurrentAccountAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = activity.ScheduleId,
                SenderId = userId,
                RecipientId = null,
                Title = $"Có ai đó vừa check-in",
                Message = $"{user!.Profile!.Name} vừa check-in: {activity.PlaceName}",
                Type = NotificationType.ActivityUpdated,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(activity.ScheduleId, new
            {
                Purpose = "Send Notification (Check-in Activity)",
                Id = notification.Id,
                ScheduleID = activity.ScheduleId,
                ScheduleName = activity.Schedule.Title,
                SenderId = userId,
                SenderName = user!.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return attendance;
        }

        public async Task<ActivityAttendance> CheckOutAsync(Guid userId, AttendanceRequest request)
        {   
            // Validate activity
            var activity = await _scheduleActivityRepository.GetActivityByIdAsync(request.ActivityId);
            if (activity == null)
            {
                throw new Exception("No activity was found");
            }
            // Validate participant
            var participant = await _participantRepository.GetByUserIdAndScheduleIdAsync(userId, activity.ScheduleId);
            if (participant == null)
            {
                throw new Exception("No participant was found");
            }


            // Validate participant in activity
            var participants = await _participantRepository.GetAllParticipantByScheduleIdAsync(activity.ScheduleId);
            if (participants == null)
            {
                throw new Exception("No participant is in the activity");
            }

            if (participants.FirstOrDefault(p => p.Id == participant.Id) == null)
            {
                throw new Exception("The participant is not in this activity");
            }

            // Validate check-in exists
            var existingCheckInAttendance = await _attendanceRepository.GetCheckInAttendanceByActivityAndParticipantAsync(request.ActivityId, participant.Id);
            if (existingCheckInAttendance == null)
            {
                throw new Exception("You need to complete the check-in before check-out");
            }

            var existingCheckOutAttendance = await _attendanceRepository.GetCheckOutAttendanceByActivityAndParticipantAsync(request.ActivityId, participant.Id);
            if(existingCheckOutAttendance != null)
            {
                throw new Exception("An activity only check-out 1 time");
            }

            var attendance = new ActivityAttendance
            {
                Id = Guid.NewGuid(),
                CheckInTime = null,
                CheckOutTime = DateTime.UtcNow,
                Status = AttendanceStatus.CheckOut,
                ActivityId = request.ActivityId,
                ParticipantId = participant.Id
            };
            await _attendanceRepository.CreateAttendanceAsync(attendance);

            // Create media
            var mediaRequest = new UploadScheduleMediaRequest
            {
                File = request.File,
                Description = request.Description,
                UploadMethod = MediaMethod.CheckOut,
                ActivityId = activity.Id,
                ScheduleId = null
            };
            await _mediaService.UploadAsync(mediaRequest);

            var user = await _authServiceClient.GetCurrentAccountAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = activity.ScheduleId,
                SenderId = userId,
                RecipientId = null,
                Title = $"Có ai đó vừa check-out",
                Message = $"{user!.Profile!.Name} vừa check-out: {activity.PlaceName}",
                Type = NotificationType.ActivityUpdated,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(activity.ScheduleId, new
            {
                Purpose = "Send Notification (Check-out Activity)",
                Id = notification.Id,
                ScheduleID = activity.ScheduleId,
                ScheduleName = activity.Schedule.Title,
                SenderId = userId,
                SenderName = user!.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });
            
            return attendance!;
        }

        public async Task<string> GetAttendanceStatusAsync(int activityId, Guid participantId)
        {
            var status = "";
            var checkin = await _attendanceRepository.GetCheckInAttendanceByActivityAndParticipantAsync(activityId, participantId);
            if (checkin != null)
            {
                status = checkin.Status.ToString();
            }

            var checkout = await _attendanceRepository.GetCheckOutAttendanceByActivityAndParticipantAsync(activityId, participantId);
            if (checkout != null)
            {
                status = checkout.Status.ToString();
            }

            return status;

        }
    }
}
