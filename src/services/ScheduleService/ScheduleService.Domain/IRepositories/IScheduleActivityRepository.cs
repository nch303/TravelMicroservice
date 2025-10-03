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
        Task<ScheduleActivity> UpdateActivityByIdAsync(ScheduleActivity newActivity, int activityId);
        Task<bool> DeleteActivityByIdAsync(int activityId);
        Task AddActivityAsync(ScheduleActivity activity);
        Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId);
        Task<ScheduleActivity?> GetActivytyByIdAsync(int id);
    }
}
