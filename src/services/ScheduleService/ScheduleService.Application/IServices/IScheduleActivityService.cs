using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;



namespace ScheduleService.Application.IServices
{
    public interface IScheduleActivityService
    {
        Task<List<ScheduleActivity>> GetAllActivitiesByScheduleIdAsync(Guid scheduleId);
        Task<ScheduleActivity> UpdateActivityById(ScheduleActivity newActivity, int activityId);
        Task DeleteActivityById(int activityId);
        Task RestoreActivityById(int activityId);
        Task AddActivityAsync(ScheduleActivity activity);
        Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId);
        Task<List<ScheduleActivity>> GetActivitiesByDateAsync(Guid scheduleId, DateTime date);
        Task UpdateOrderIndexById(int newIndex, int activityId);
        //Task<Dictionary<string, List<ScheduleActivity>>> GetActivitiesGroupedByDateAsync(Guid scheduleId, DateTime date);
        Task<List<ScheduleActivity>> AddListActivityAsync(List<ScheduleActivity> scheduleActivities);
    }
}

