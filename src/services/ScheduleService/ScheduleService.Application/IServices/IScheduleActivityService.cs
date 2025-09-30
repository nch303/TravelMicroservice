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
        Task<List<ScheduleActivity>> GetActiviyListByScheduleId(Guid scheduleId);
        Task<ScheduleActivity> UpdateActivityById(ScheduleActivity newActivity, int activityId);
        Task DeleteActivityById(int activityId);
    }
}
