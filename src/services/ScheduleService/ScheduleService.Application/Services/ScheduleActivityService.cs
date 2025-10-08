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

namespace ScheduleService.Application.Services
{
    public class ScheduleActivityService : IScheduleActivityService
    {
        private readonly IScheduleActivityRepository _scheduleActivityRepository;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;

        public ScheduleActivityService(IScheduleActivityRepository scheduleActivityRepository,IScheduleParticipantRepository scheduleParticipantRepository,
            IScheduleRepository scheduleRepository, IAuthServiceClient authServiceClient)
        {
            _scheduleActivityRepository = scheduleActivityRepository;
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _scheduleRepository = scheduleRepository;
            _authServiceClient = authServiceClient;
        }

        private static void ValidateActivityOrder(List<ScheduleActivity> activities, ScheduleActivity current)
        {
            if (current == null)
                throw new ArgumentNullException(nameof(current));

            if (activities == null || activities.Count == 0)
                return;

            // Sort all activities by CheckInTime for consistent order
            var ordered = activities
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.CheckInTime)
                .ToList();

            // Find the current activity's position in the ordered list
            var index = ordered.FindIndex(a => a.Id == current.Id);
            if (index == -1)
                throw new ArgumentException("The current activity does not exist in the provided list.");

            // 1️⃣ Validate the activity itself (CheckIn < CheckOut)
            if (current.CheckOutTime <= current.CheckInTime)
                throw new ArgumentException("Check-out time must be later than check-in time.");

            // 2️⃣ If this is the first activity, only validate with the next one (no previous)
            if (index == 0)
            {
                if (ordered.Count > 1)
                {
                    var next = ordered[1];
                    if (current.CheckOutTime > next.CheckInTime)
                    {
                        throw new ArgumentException(
                            $"The first activity '{current.PlaceName}' ends after the next activity starts. " +
                            $"Next check-in: {next.CheckInTime:t}, current checkout: {current.CheckOutTime:t}");
                    }
                }
                return; // ✅ No need to check previous; done for first activity
            }

            // 3️⃣ Validate with the previous activity (ensure no overlap)
            var previous = ordered[index - 1];
            if (current.CheckInTime < previous.CheckOutTime)
            {
                throw new ArgumentException(
                    $"Activity '{current.PlaceName}' starts before the previous activity ends. " +
                    $"Previous checkout: {previous.CheckOutTime:t}, current check-in: {current.CheckInTime:t}");
            }

            // 4️⃣ Validate with the next activity (ensure no overlap)
            if (index < ordered.Count - 1)
            {
                var next = ordered[index + 1];
                if (current.CheckOutTime > next.CheckInTime)
                {
                    throw new ArgumentException(
                        $"Activity '{current.PlaceName}' ends after the next activity starts. " +
                        $"Next check-in: {next.CheckInTime:t}, current checkout: {current.CheckOutTime:t}");
                }
            }
        }


        public async Task<ScheduleActivity> UpdateActivityById(ScheduleActivity newActivity, int activityId)
        {
            // 1️⃣ Find the existing activity
            var existing = await _scheduleActivityRepository.GetActivityByIdAsync(activityId);
            if (existing == null)
                throw new KeyNotFoundException($"Activity with Id {activityId} not found.");

            // 2️⃣ Validate basic time logic
            if (newActivity.CheckInTime >= newActivity.CheckOutTime)
                throw new ArgumentException("Check-in time must be earlier than check-out time.");

            // 3️⃣ Fetch all activities in the same schedule
            var allActivities = await _scheduleActivityRepository.GetActivitiesByScheduleIdAsync(existing.ScheduleId);

            // Include existing (to validate against all others)
            var updatedList = allActivities
                .Where(a => !a.IsDeleted)
                .ToList();

            // 4️⃣ Temporarily apply new times to existing (for validation only)
            var tempActivity = new ScheduleActivity
            {
                Id = existing.Id,
                ScheduleId = existing.ScheduleId,
                PlaceName = newActivity.PlaceName,
                Location = newActivity.Location,
                Description = newActivity.Description,
                CheckInTime = newActivity.CheckInTime,
                CheckOutTime = newActivity.CheckOutTime
            };

            // Replace existing activity with the temporary updated one for validation
            int index = updatedList.FindIndex(a => a.Id == existing.Id);
            if (index != -1)
                updatedList[index] = tempActivity;

            // 5️⃣ Validate time overlap consistency
            ValidateActivityOrder(updatedList, tempActivity);

            // 6️⃣ Update fields (only after successful validation)
            existing.PlaceName = newActivity.PlaceName;
            existing.Location = newActivity.Location;
            existing.Description = newActivity.Description;
            existing.CheckInTime = newActivity.CheckInTime;
            existing.CheckOutTime = newActivity.CheckOutTime;

            // 7️⃣ Recalculate order indexes based on CheckInTime
            var ordered = updatedList
                .OrderBy(a => a.CheckInTime)
                .ThenBy(a => a.CheckOutTime)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].OrderIndex = i + 1;
            }

            // 8️⃣ Save changes
            var result = await _scheduleActivityRepository.SaveChangesAsync();
            if (result <= 0)
                throw new InvalidOperationException("Failed to update activity and reorder schedule.");

            return existing;
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
            var remainingActivities = await _scheduleActivityRepository.GetActivitiesByScheduleIdAsync(activity.ScheduleId);

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
        }

        public async Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
            if (participant == null)
            {
                throw new Exception("You do not have permission to view activities of this schedule");
            }

            var activities = await _scheduleActivityRepository.GetActivitiesByScheduleIdAsync(scheduleId);
            return activities;
        }
    }
}
