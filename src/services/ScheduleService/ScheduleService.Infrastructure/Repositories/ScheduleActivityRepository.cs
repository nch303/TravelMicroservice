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
    public class ScheduleActivityRepository : IScheduleActivityRepository
    {
        private readonly AppDbContext _context;
        public ScheduleActivityRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleActivity> UpdateActivityByIdAsync(ScheduleActivity newActivity, int activityId)
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

        public async Task<bool> DeleteActivityByIdAsync(int activityId)
        {
            var activity = await _context.ScheduleActivities
                .FirstOrDefaultAsync(a => a.Id == activityId && !a.IsDeleted);

            if (activity == null)
                return false; // activity not found

            activity.IsDeleted = true;

            var result = await _context.SaveChangesAsync();
            return result > 0;
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
