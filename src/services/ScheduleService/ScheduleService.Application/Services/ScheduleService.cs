using ScheduleService.Application.DTOs.Responses;
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
    public class SchedulesService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public SchedulesService(IScheduleRepository scheduleRepository
            , IScheduleParticipantRepository scheduleParticipantRepository
            , IAuthServiceClient authServiceClient, INotificationService notificationService, IRealtimeNotifier realtimeNotifier)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _authServiceClient = authServiceClient;
            _notificationService = notificationService;
            _realtimeNotifier = realtimeNotifier;
        }

        private DateTime ConvertToUtc7(DateTime localDateTime)
        {
            // Convert sang giờ VN (UTC+7)
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(localDateTime, vnTimeZone);
            return localTime;
        }

        private static void ValidateSchedule(Schedule validateSchedule, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(validateSchedule.Title))
                throw new ArgumentException("Title is required");
            if (validateSchedule.Title.Length > 50)
                throw new ArgumentException("Title must be at most 255 characters");

            if (!string.IsNullOrEmpty(validateSchedule.StartLocation) && validateSchedule.StartLocation.Length > 255)
                throw new ArgumentException("StartLocation must be at most 255 characters");

            if (!string.IsNullOrEmpty(validateSchedule.Destination) && validateSchedule.Destination.Length > 255)
                throw new ArgumentException("Destination must be at most 255 characters");

            if (validateSchedule.EndDate != default && validateSchedule.StartDate != default && validateSchedule.EndDate < validateSchedule.StartDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate");

            // Only validate ParticipantsCount if it's explicitly provided or we're creating
            if (!isUpdate || validateSchedule.ParticipantsCount != default)
            {
                if (validateSchedule.ParticipantsCount < 0)
                    throw new ArgumentException("ParticipantsCount must be non-negative");
            }
        }

        public async Task<Schedule> GetScheduleByIdAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            return schedule;
        }

        public async Task SaveChangesAsync()
        {
            await _scheduleRepository.SaveChangesAsync();
        }

        public async Task<string> ShareScheduleAsync(Guid id)
        {
            var schedule = await GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            // Logic to generate a share code
            var sharedCode = schedule.GenerateRandomCode();
            schedule.SharedCode = sharedCode;
            await SaveChangesAsync();
            return sharedCode;
        }

        public async Task JoinScheduleAsync(string sharedCode, AccountResponse user)
        {
            var schedule = await _scheduleRepository.GetScheduleByShareCodeAsync(sharedCode);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = schedule.Id,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Có thành viên mới gia nhập",
                Message = $"Nhóm {schedule.Title} vừa có thêm thành viên mới",
                Type = NotificationType.ScheduleJoined,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            var existingParticipant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user.Id, schedule.Id);
            if (existingParticipant != null)
            {
                if (existingParticipant.Status == ParticipantStatus.Banned)
                {
                    throw new Exception("You have been banned from this schedule. To re-join, contact to the onwer to be restored.");
                }
                else if (existingParticipant.Status == ParticipantStatus.Active)
                {
                    throw new Exception("You are already a participant in this schedule");
                }
                else if (existingParticipant.Status == ParticipantStatus.Left)
                {
                    existingParticipant.Status = ParticipantStatus.Active;
                    existingParticipant.JoineddAt = DateTime.UtcNow;
                    await _scheduleParticipantRepository.SaveChangesAsync();

                    // Increase participant count
                    schedule.ParticipantsCount += 1;
                    await _scheduleRepository.SaveChangesAsync();

                    // Send Notification Realtime
                    await _realtimeNotifier.SendGroupNotificationAsync(schedule.Id, new
                    {
                        Purpose = "Send Notification (New Joiner)",
                        Id = notification.Id,
                        ScheduleID = schedule.Id,
                        ScheduleName = schedule.Title,
                        SenderId = user.Id,
                        SenderName = user.Profile!.Name,
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = notification.Type.ToString(),
                        CreatedAt = ConvertToUtc7(notification.CreatedAt)
                    });

                    return;
                }
            }

            var participant = new ScheduleParticipant
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ScheduleId = schedule.Id,
                Role = ParticipantRole.Viewer,
                JoineddAt = DateTime.UtcNow,
                Status = ParticipantStatus.Active
            };

            // Increase participant count
            schedule.ParticipantsCount += 1;
            await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);

            

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(schedule.Id, new
            {
                Purpose = "Send Notification (New Joiner)",
                Id = notification.Id,
                ScheduleID = schedule.Id,
                ScheduleName = schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            await _scheduleRepository.SaveChangesAsync();
        }

        public async Task<Schedule> UpdateScheduleByIdAsync(Schedule newSchedule, Guid id)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, id);
            if (participant!.Role != ParticipantRole.Owner && participant.Role != ParticipantRole.Editor)
            {
                throw new Exception("You do not have permission to update this schedule");
            }

            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with Id {id} not found.");
            }

            // Validate the new data first
            ValidateSchedule(newSchedule, isUpdate: true);

            // Update fields
            schedule.Title = newSchedule.Title;
            schedule.StartLocation = newSchedule.StartLocation;
            schedule.Destination = newSchedule.Destination;
            schedule.StartDate = newSchedule.StartDate;
            schedule.EndDate = newSchedule.EndDate;
            schedule.Notes = newSchedule.Notes;
            schedule.IsShared = newSchedule.IsShared;

            // Always update UpdatedAt
            schedule.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _scheduleRepository.SaveChangesAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = schedule.Id,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {schedule.Title} vừa có 1 thay đổi",
                Type = NotificationType.ScheduleUpdated,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(schedule.Id, new
            {
                Purpose = "Send Notification (Update Schedule)",
                Id = notification.Id,
                ScheduleID = schedule.Id,
                ScheduleName = schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return schedule;
        }

        public async Task<bool> CancelScheduleAsync(Guid id, AccountResponse user)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with Id {id} not found.");
            }

            if (user.Id != schedule.OwnerId)
            {
                throw new Exception("You do not have permission.");
            }

            schedule.Status = ScheduleStatus.Inactive;
            // Save changes
            await _scheduleRepository.SaveChangesAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = schedule.Id,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {schedule.Title} đã bị hủy",
                Type = NotificationType.ScheduleDeleted,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(schedule.Id, new
            {
                Purpose = "Send Notification (Delete Schedule)",
                Id = notification.Id,
                ScheduleID = schedule.Id,
                ScheduleName = schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return true;
        }

        public async Task<bool> RestoreScheduleAsync(Guid id, AccountResponse user)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with Id {id} not found.");
            }

            if (user.Id != schedule.OwnerId)
            {
                throw new Exception("You do not have permission.");
            }

            schedule.Status = ScheduleStatus.Active;
            // Save changes
            await _scheduleRepository.SaveChangesAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = schedule.Id,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {schedule.Title} đã được khôi phục",
                Type = NotificationType.ScheduleRestored,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(schedule.Id, new
            {
                Purpose = "Send Notification (Restore Schedule)",
                Id = notification.Id,
                ScheduleID = schedule.Id,
                ScheduleName = schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return true;
        }

        public async Task CreateScheduleAsync(Schedule schedule)
        {
            ValidateSchedule(schedule, isUpdate: false);
            await _scheduleRepository.CreateScheduleAsync(schedule);
            await _scheduleRepository.SaveChangesAsync();
        }
    }
}
