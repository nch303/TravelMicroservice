using CloudinaryDotNet.Actions;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Application.ServiceClients;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;
using Sprache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Services
{
    public class ScheduleParticipantService : IScheduleParticipantService
    {
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly INotificationService _notificationService;

        public ScheduleParticipantService(IScheduleParticipantRepository scheduleParticipantRepository, IScheduleRepository scheduleRepository
            , IUserServiceClient userServiceClient, IAuthServiceClient authServiceClient
            , IRealtimeNotifier realtimeNotifier, INotificationService notificationService)
        {
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _scheduleRepository = scheduleRepository;
            _userServiceClient = userServiceClient;
            _authServiceClient = authServiceClient;
            _realtimeNotifier = realtimeNotifier;
            _notificationService = notificationService;
        }

        private DateTime ConvertToUtc7(DateTime localDateTime)
        {
            // Convert sang giờ VN (UTC+7)
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(localDateTime, vnTimeZone);
            return localTime;
        }


        public async Task<ScheduleParticipant?> GetByUserIdAndScheduleIdAsync(Guid userId, Guid scheduleId)
        {
            var schedule = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(userId, scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule participant not found");
            }
            return schedule;
        }

        public async Task<List<ScheduleParticipant>> GetAllScheduleByParticipantIdAsync(Guid participantId)
        {
            var schedules = await _scheduleParticipantRepository.GetAllScheduleByParticipantIdAsync(participantId);
            return schedules ?? new List<ScheduleParticipant>();
        }

        public async Task<Schedule?> LeaveScheduleAsync(Guid scheduleId)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
            if (participant == null || participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("User is not an active participant.");

            participant.Status = ParticipantStatus.Left;
            participant.JoineddAt = DateTime.UtcNow;

            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            if (schedule.ParticipantsCount > 0)
                schedule.ParticipantsCount--;

            schedule.UpdatedAt = DateTime.UtcNow;

            await _scheduleRepository.SaveChangesAsync();
            await _scheduleParticipantRepository.SaveChangesAsync();

            // Reload the schedule with participants included
            var updatedSchedule = await _scheduleRepository.GetScheduleWithParticipantsByIdAsync(scheduleId);

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = scheduleId,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {schedule.Title} có 1 người rời đi",
                Type = NotificationType.ScheduleLeft,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(scheduleId, new
            {
                Purpose = "Send Notification (Leave Schedule)",
                Id = notification.Id,
                ScheduleID = scheduleId,
                ScheduleName = schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return updatedSchedule;
        }

        public async Task<ScheduleParticipant> ChangeParticipantRoleAsync(Guid userId, Guid scheduleId)
        {
            // Get current user (the one making the request)
            var currentUser = await _authServiceClient.GetCurrentAccountAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            // Get the schedule
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            // Ensure current user is the owner
            if (schedule.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("Only the schedule owner can change participant's role.");

            // Get the participant to change role
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(userId, scheduleId);
            if (participant == null)
                throw new KeyNotFoundException("Participant not found.");
            if (participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("Participant is not currently active.");

            // Mark participant as 'Editor'
            if (participant.Role == ParticipantRole.Viewer)
            {
                participant.Role = ParticipantRole.Editor;
            }
            else if (participant.Role == ParticipantRole.Editor)
            {
                participant.Role = ParticipantRole.Viewer;
            }

            schedule.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _scheduleParticipantRepository.SaveChangesAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = null,
                SenderId = currentUser.Id,
                RecipientId = userId,
                Title = $" Bạn có 1 cập nhập mới",
                Message = $"Bạn đã là Editor của nhóm {schedule.Title}",
                Type = NotificationType.ScheduleRoleChanged,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(scheduleId, new
            {
                Purpose = "Send Notification (Change Role Schedule)",
                Id = notification.Id,
                ScheduleID = scheduleId,
                ScheduleName = schedule.Title,
                SenderId = currentUser.Id,
                SenderName = currentUser.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return participant;
        }

        public async Task<Schedule?> KickParticipantAsync(Guid scheduleId, Guid participantId)
        {
            // Get current user (the one making the request)
            var currentUser = await _authServiceClient.GetCurrentAccountAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            // Get the schedule
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            // Ensure current user is the owner
            if (schedule.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("Only the schedule owner can kick participants.");

            // Get the participant to remove
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(participantId, scheduleId);
            if (participant == null)
                throw new KeyNotFoundException("Participant not found.");
            if (participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("Participant is not currently active.");

            // Owner cannot kick themselves
            if (participant.UserId == currentUser.Id)
                throw new InvalidOperationException("Owner cannot kick themselves from the schedule.");

            // Mark participant as 'Banned'
            participant.Status = ParticipantStatus.Banned;

            // Decrease participant count safely
            if (schedule.ParticipantsCount > 0)
                schedule.ParticipantsCount--;

            schedule.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _scheduleParticipantRepository.SaveChangesAsync();
            await _scheduleRepository.SaveChangesAsync();

            // Reload the updated schedule with participants
            var updatedSchedule = await _scheduleRepository.GetScheduleWithParticipantsByIdAsync(scheduleId);

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = scheduleId,
                SenderId = currentUser.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {schedule.Title} có 1 người bị cấm",
                Type = NotificationType.ScheduleBanned,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(scheduleId, new
            {
                Purpose = "Send Notification (Leave Schedule)",
                Id = notification.Id,
                ScheduleID = scheduleId,
                ScheduleName = schedule.Title,
                SenderId = currentUser.Id,
                SenderName = currentUser.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return updatedSchedule;
        }

        public async Task<ScheduleParticipant> AddScheduleParticipantAsync(ScheduleParticipant participant)
        {
            var newParticipant = await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);
            await _scheduleParticipantRepository.SaveChangesAsync();
            return newParticipant;
        }

        public async Task<ScheduleParticipant> AddParticipantByEmailAsync(Guid scheduleId, string email)
        {
            var result = new ScheduleParticipant();
            // 1) Ensure current user is owner of the schedule
            var currentUser = await _authServiceClient.GetCurrentAccountAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            if (schedule.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("User has no permission");

            // 2) Validate email exists and is active via AuthService
            var account = await _authServiceClient.GetAccountByEmailAsync(email);
            if (account == null || !account.IsActive)
                throw new Exception("email is invalid");

            // 3) Check duplication
            var existing = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(account.Id, scheduleId);
            if (existing != null && existing.Status == ParticipantStatus.Active)
                throw new Exception("This user is already in the schedule");

            // 4) Create or reactivate participant
            if (existing != null && existing.Status != ParticipantStatus.Active)
            {
                existing.Status = ParticipantStatus.Active;
                existing.JoineddAt = DateTime.UtcNow;
                await _scheduleParticipantRepository.SaveChangesAsync();

                // Update schedule participant count
                schedule.ParticipantsCount++;
                schedule.UpdatedAt = DateTime.UtcNow;
                await _scheduleRepository.SaveChangesAsync();

                result = existing;
            }
            else
            {
                var participant = new ScheduleParticipant
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    UserId = account.Id,
                    Role = ParticipantRole.Viewer,
                    Status = ParticipantStatus.Active,
                    JoineddAt = DateTime.UtcNow
                };

                var created = await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);

                // Update schedule participant count
                schedule.ParticipantsCount++;
                schedule.UpdatedAt = DateTime.UtcNow;
                await _scheduleRepository.SaveChangesAsync();
                result = created;
            }

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = scheduleId,
                SenderId = currentUser.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {schedule.Title} có 1 người tham gia",
                Type = NotificationType.ScheduleJoined,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(scheduleId, new
            {
                Purpose = "Send Notification (Join Schedule)",
                Id = notification.Id,
                ScheduleID = scheduleId,
                ScheduleName = schedule.Title,
                SenderId = currentUser.Id,
                SenderName = currentUser.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return result;
        }

        public async Task<(List<ScheduleParticipant> Participants, List<UserServiceClientResponse> Users)>
    GetAllParticipantByScheduleIdAsync(Guid scheduleId)
        {
            var participants = await _scheduleParticipantRepository.GetAllParticipantByScheduleIdAsync(scheduleId);
            var userIds = participants.Select(p => p.UserId).Distinct().ToList();
            var users = await _userServiceClient.GetUsersByIdsAsync(userIds);

            if (participants == null || !participants.Any() || users == null)
            {
                var emptyParticipant = new List<ScheduleParticipant>();
                var emptyUser = new List<UserServiceClientResponse>();
                return (emptyParticipant, emptyUser);
            }

            return (participants, users);
        }
    }
}
