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
        Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId);
        Task<ScheduleActivity?> GetActivityByIdAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
