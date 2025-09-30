using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;

namespace ScheduleService.Application.Services
{
    public class ScheduleActivityService : IScheduleActivityService
    {
        private readonly IScheduleActivityRepository _scheduleActivitiesRepository;

        public ScheduleActivityService(IScheduleActivityRepository scheduleActivitiesRepository)
        {
            _scheduleActivitiesRepository = scheduleActivitiesRepository;
        }

        public async Task<List<ScheduleActivity>> GetActiviyListByScheduleId(Guid scheduleId)
        {
            return await _scheduleActivitiesRepository.GetActiviyListByScheduleId(scheduleId);
        }

        public async Task<ScheduleActivity> UpdateActivityById(ScheduleActivity newActivity, int activityId)
        {
            var updated = await _scheduleActivitiesRepository.UpdateActivityById(newActivity, activityId);
            if (updated == null)
                throw new KeyNotFoundException("Activity not found");

            return updated;
        }

        public async Task DeleteActivityById(int activityId)
        {
            await _scheduleActivitiesRepository.DeleteActivityById(activityId);
        }
    }
}
