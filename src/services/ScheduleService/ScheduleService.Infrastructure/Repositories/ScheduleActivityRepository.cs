using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ScheduleService.Infrastructure.Repositories
{
    public class ScheduleActivityRepository : IScheduleAcitvitiyRepository
    {
        private readonly AppDbContext _context;
        public ScheduleActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScheduleActivity>> GetActiviyListByScheduleId(Guid scheduleId)
        {
            return await _context.ScheduleActivities
                .Where(a => a.ScheduleId == scheduleId && !a.IsDeleted)
                .OrderBy(a => a.OrderIndex)
                .ToListAsync();
        }

        public async Task<ScheduleActivity> UpdateActivityById(ScheduleActivity newActivity, int activityId)
        {
            var existing = await _context.ScheduleActivities
               .FirstOrDefaultAsync(a => a.Id == activityId && !a.IsDeleted);

            if (existing == null)
                throw new KeyNotFoundException("Activity not found");

            // update fields
            existing.PlaceName = newActivity.PlaceName;
            existing.Location = newActivity.Location;
            existing.Description = newActivity.Description;
            existing.CheckInTime = newActivity.CheckInTime;
            existing.CheckOutTime = newActivity.CheckOutTime;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteActivityById(int activityId)
        {
            var activity = await _context.ScheduleActivities
                .FirstOrDefaultAsync(a => a.Id == activityId && !a.IsDeleted);

            if (activity == null)
                throw new KeyNotFoundException("Activity not found");

            activity.IsDeleted = true; 
            await _context.SaveChangesAsync();
        }

        public async Task AddActivityAsync(ScheduleActivity activity)
        {
            await _context.ScheduleActivities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }


        public async Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.ScheduleActivities
                .Where(sa => sa.ScheduleId == scheduleId && !sa.IsDeleted)
                .OrderBy(sa => sa.OrderIndex)
                .ToListAsync();
        }
    }
}
