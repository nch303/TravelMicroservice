using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Domain.IRepositories
{
    public interface IScheduleActivityRepository
    {
        Task AddActivityAsync(ScheduleActivity activity);
        Task<List<ScheduleActivity>> GetAllActivitiesByScheduleIdAsync(Guid scheduleId);
        Task<List<ScheduleActivity>> GetAvailableActivitiesByScheduleIdAsync(Guid scheduleId);
        Task<ScheduleActivity?> GetActivityByIdAsync(int id);
        Task<int> SaveChangesAsync();
        Task<ScheduleActivity?> GetDeletedActivityByIdAsync(int id);
        Task<List<ScheduleActivity>> GetActivitiesByDateAsync(Guid scheduleId, DateTime date);
}
