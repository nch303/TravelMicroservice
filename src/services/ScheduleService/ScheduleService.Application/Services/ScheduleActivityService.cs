using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Application.ServiceClients;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScheduleService.Application.Services
{
    public class ScheduleActivityService : IScheduleActivityService
    {
        private readonly IScheduleActivityRepository _scheduleActivityRepository;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        private readonly INotificationService _notificationService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public ScheduleActivityService(IScheduleActivityRepository scheduleActivityRepository,IScheduleParticipantRepository scheduleParticipantRepository,
            IScheduleRepository scheduleRepository, IAuthServiceClient authServiceClient, INotificationService notificationService
            , IRealtimeNotifier realtimeNotifier)
        {
            _scheduleActivityRepository = scheduleActivityRepository;
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _scheduleRepository = scheduleRepository;
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

        //private static void ValidateActivityOrder(List<ScheduleActivity> activities, ScheduleActivity current)
        //{
        //    if (current == null)
        //        throw new ArgumentNullException(nameof(current));

        //    if (activities == null || activities.Count == 0)
        //        return;

        //    // Sort all activities by CheckInTime for consistent order
        //    var ordered = activities
        //        .Where(a => !a.IsDeleted)
        //        .OrderBy(a => a.CheckInTime)
        //        .ToList();

        //    // Find the current activity's position in the ordered list
        //    var index = ordered.FindIndex(a => a.Id == current.Id);
        //    if (index == -1)
        //        throw new ArgumentException("The current activity does not exist in the provided list.");

        //    // 1️⃣ Validate the activity itself (CheckIn < CheckOut)
        //    if (current.CheckOutTime <= current.CheckInTime)
        //        throw new ArgumentException("Check-out time must be later than check-in time.");

        //    // 2️⃣ If this is the first activity, only validate with the next one (no previous)
        //    if (index == 0)
        //    {
        //        if (ordered.Count > 1)
        //        {
        //            var next = ordered[1];
        //            if (current.CheckOutTime > next.CheckInTime)
        //            {
        //                throw new ArgumentException(
        //                    $"The first activity '{current.PlaceName}' ends after the next activity starts. " +
        //                    $"Next check-in: {next.CheckInTime:t}, current checkout: {current.CheckOutTime:t}");
        //            }
        //        }
        //        return; // ✅ No need to check previous; done for first activity
        //    }

        //    // 3️⃣ Validate with the previous activity (ensure no overlap)
        //    var previous = ordered[index - 1];
        //    if (current.CheckInTime < previous.CheckOutTime)
        //    {
        //        throw new ArgumentException(
        //            $"Activity '{current.PlaceName}' starts before the previous activity ends. " +
        //            $"Previous checkout: {previous.CheckOutTime:t}, current check-in: {current.CheckInTime:t}");
        //    }

        //    // 4️⃣ Validate with the next activity (ensure no overlap)
        //    if (index < ordered.Count - 1)
        //    {
        //        var next = ordered[index + 1];
        //        if (current.CheckOutTime > next.CheckInTime)
        //        {
        //            throw new ArgumentException(
        //                $"Activity '{current.PlaceName}' ends after the next activity starts. " +
        //                $"Next check-in: {next.CheckInTime:t}, current checkout: {current.CheckOutTime:t}");
        //        }
        //    }
        //}

        public async Task<List<ScheduleActivity>> GetAllActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            var activities = await _scheduleActivityRepository.GetAllActivitiesByScheduleIdAsync(scheduleId);
            if (activities == null || !activities.Any())
                return new List<ScheduleActivity>();

            return activities;
        }

        public async Task<List<ScheduleActivity>> GetActivitiesByDateAsync(Guid scheduleId, DateTime date)
        {
            var activities = await _scheduleActivityRepository.GetActivitiesByDateAsync(scheduleId, date);

            if (activities == null || !activities.Any())
                return new List<ScheduleActivity>();

            return activities;
        }

        public async Task<ScheduleActivity> UpdateActivityById(ScheduleActivity newActivity, int activityId)
        {
            // Find the existing activity
            var existing = await _scheduleActivityRepository.GetActivityByIdAsync(activityId);
            if (existing == null)
                throw new KeyNotFoundException($"Activity with Id {activityId} not found.");

            var user = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, existing.ScheduleId);
            if ((participant!.Role != ParticipantRole.Owner && participant.Role != ParticipantRole.Editor) || participant == null)
            {
                throw new Exception("You do not have permission to update this schedule");
            }

            // Validate basic time logic
            if (newActivity.CheckInTime >= newActivity.CheckOutTime)
                throw new ArgumentException("Check-in time must be earlier than check-out time.");

            // Fetch all activities in the same schedule
            //var allActivities = await _scheduleActivityRepository.GetActivitiesByScheduleIdAsync(existing.ScheduleId);

            // Include existing (to validate against all others)
            //var updatedList = allActivities
            //    .Where(a => !a.IsDeleted)
            //    .ToList();

            // Temporarily apply new times to existing (for validation only)
            //var tempActivity = new ScheduleActivity
            //{
            //    Id = existing.Id,
            //    ScheduleId = existing.ScheduleId,
            //    PlaceName = newActivity.PlaceName,
            //    Location = newActivity.Location,
            //    Description = newActivity.Description,
            //    CheckInTime = newActivity.CheckInTime,
            //    CheckOutTime = newActivity.CheckOutTime
            //};

            // Replace existing activity with the temporary updated one for validation
            //int index = updatedList.FindIndex(a => a.Id == existing.Id);
            //if (index != -1)
            //    updatedList[index] = tempActivity;

            // Validate time overlap consistency
            //ValidateActivityOrder(updatedList, tempActivity);

            // Update fields (only after successful validation)
            existing.PlaceName = newActivity.PlaceName;
            existing.Location = newActivity.Location;
            existing.Description = newActivity.Description;
            existing.CheckInTime = newActivity.CheckInTime;
            existing.CheckOutTime = newActivity.CheckOutTime;

            //  Save changes
            var result = await _scheduleActivityRepository.SaveChangesAsync();
            if (result <= 0)
                throw new InvalidOperationException("Failed to update activity and reorder schedule.");

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = existing.ScheduleId,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {existing.Schedule.Title} có 1 thay đổi hoạt động",
                Type = NotificationType.ActivityUpdated,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(existing.ScheduleId, new
            {
                Purpose = "Send Notification (Update Activity)",
                Id = notification.Id,
                ScheduleID = existing.ScheduleId,
                ScheduleName = existing.Schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });

            return existing;
        }

        public async Task UpdateOrderIndexById(int newIndex, int activityId)
        {
            var existing = await _scheduleActivityRepository.GetActivityByIdAsync(activityId);
            if (existing == null)
                throw new KeyNotFoundException($"Activity with Id {activityId} not found.");

            existing.OrderIndex = newIndex;

            // 8️⃣ Save changes
            var result = await _scheduleActivityRepository.SaveChangesAsync();
            if (result <= 0)
                throw new InvalidOperationException("Failed to update activity and reorder schedule.");

            var user = await _authServiceClient.GetCurrentAccountAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = existing.ScheduleId,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {existing.Schedule.Title} có 1 thay đổi về thứ tự hoạt động",
                Type = NotificationType.ActivityUpdated,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(existing.ScheduleId, new
            {
                Purpose = "Send Notification (Update Activity)",
                Id = notification.Id,
                ScheduleID = existing.ScheduleId,
                ScheduleName = existing.Schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });
        }

        public async Task DeleteActivityById(int activityId)
        {
            // 1️⃣ Get the target activity
            var activity = await _scheduleActivityRepository.GetActivityByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Activity with that Id not found or already deleted.");

            // 2️⃣ Soft delete
            activity.IsDeleted = true;

            // 3️⃣ Get other remaining activities in the same schedule
            var remainingActivities = await _scheduleActivityRepository.GetAvailableActivitiesByScheduleIdAsync(activity.ScheduleId);

            // 4️⃣ Reorder: shift up any activities after the deleted one
            foreach (var a in remainingActivities)
            {
                if (a.OrderIndex > activity.OrderIndex)
                    a.OrderIndex -= 1;
            }

            // 5️⃣ Save changes
            var result = await _scheduleActivityRepository.SaveChangesAsync();

            if (result <= 0)
                throw new InvalidOperationException("Failed to delete activity and update order indexes.");

            var user = await _authServiceClient.GetCurrentAccountAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = activity.ScheduleId,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {activity.Schedule.Title} có 1 hoạt động bị hủy",
                Type = NotificationType.ActivityDeleted,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(activity.ScheduleId, new
            {
                Purpose = "Send Notification (Delete Activity)",
                Id = notification.Id,
                ScheduleID = activity.ScheduleId,
                ScheduleName = activity.Schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });
        }

        public async Task RestoreActivityById(int activityId)
        {
            var activity = await _scheduleActivityRepository.GetDeletedActivityByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Activity with that Id not found or is active.");

            var activeActivities = await _scheduleActivityRepository.GetAvailableActivitiesByScheduleIdAsync(activity.ScheduleId);
            var nextOrderIndex = activeActivities.Any()
                    ? activeActivities.Max(a => a.OrderIndex) + 1
                    : 1;

            activity.OrderIndex = nextOrderIndex;
            activity.IsDeleted = false;

            var result = await _scheduleActivityRepository.SaveChangesAsync();

            if (result <= 0)
                throw new InvalidOperationException("Failed to restore activity and update order indexes.");

            var user = await _authServiceClient.GetCurrentAccountAsync();

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = activity.ScheduleId,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {activity.Schedule.Title} có 1 thay đổi về hoạt động",
                Type = NotificationType.ActivityRestored,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(activity.ScheduleId, new
            {
                Purpose = "Send Notification (Restore Activity)",
                Id = notification.Id,
                ScheduleID = activity.ScheduleId,
                ScheduleName = activity.Schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });
        }

        public async Task AddActivityAsync(ScheduleActivity activity)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(activity.ScheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, activity.ScheduleId);
            if (participant == null || (participant.Role != ParticipantRole.Owner && participant.Role != ParticipantRole.Editor))
            {
                throw new Exception("You do not have permission to add activity to this schedule");
            }
            await _scheduleActivityRepository.AddActivityAsync(activity);

            // Create notification
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                ScheduleId = activity.ScheduleId,
                SenderId = user.Id,
                RecipientId = null,
                Title = $"Lịch trình có 1 cập nhập",
                Message = $"Nhóm {activity.Schedule.Title} có 1 thay đổi về hoạt động",
                Type = NotificationType.ActivityCreated,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationService.CreateNotificationAsync(notification);

            // Send Notification Realtime
            await _realtimeNotifier.SendGroupNotificationAsync(activity.ScheduleId, new
            {
                Purpose = "Send Notification (Create Activity)",
                Id = notification.Id,
                ScheduleID = activity.ScheduleId,
                ScheduleName = activity.Schedule.Title,
                SenderId = user.Id,
                SenderName = user.Profile!.Name,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),
                CreatedAt = ConvertToUtc7(notification.CreatedAt)
            });
        }

        public async Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            //var user = await _authServiceClient.GetCurrentAccountAsync();
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            //var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
            //if (participant == null)
            //{
            //    throw new Exception("You do not have permission to view activities of this schedule");
            //}

            var activities = await _scheduleActivityRepository.GetAvailableActivitiesByScheduleIdAsync(scheduleId);
            if (activities == null || !activities.Any())
                return new List<ScheduleActivity>();
            var sorted = activities.OrderBy(a => a.OrderIndex).ToList();
            return sorted;
        }
    }
}
