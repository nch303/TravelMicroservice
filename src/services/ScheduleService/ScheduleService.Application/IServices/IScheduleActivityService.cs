using ScheduleService.Domain.Entities;

namespace ScheduleService.Application.IServices
{
    public interface IScheduleActivityService
    {
        Task AddActivityAsync(ScheduleActivity activity);
        Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId);
    }
}