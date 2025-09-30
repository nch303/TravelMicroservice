using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;

namespace ScheduleService.Infrastructure.Repositories
{
    public class ScheduleActivityRepository : IScheduleActivityRepository
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
    }
}
