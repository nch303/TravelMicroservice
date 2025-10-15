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
        
        public async Task AddActivityAsync(ScheduleActivity activity)
        {
            await _context.ScheduleActivities.AddAsync(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScheduleActivity>> GetAvailableActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.ScheduleActivities
                .Where(sa => sa.ScheduleId == scheduleId && !sa.IsDeleted)
                .OrderBy(sa => sa.OrderIndex)
                .ToListAsync();
        }

        public async Task<List<ScheduleActivity>> GetActivitiesByDateAsync(Guid scheduleId, DateTime date)
        {
            return await _context.ScheduleActivities
                .Where(a => a.ScheduleId == scheduleId
                    && !a.IsDeleted
                    && a.CheckInTime.Date == date.Date)
                .OrderBy(a => a.OrderIndex)
                .ToListAsync();
        }


        public async Task<List<ScheduleActivity>> GetAllActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.ScheduleActivities
                .Where(sa => sa.ScheduleId == scheduleId)
                .OrderBy(sa => sa.OrderIndex)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<ScheduleActivity?> GetActivityByIdAsync(int id)
        {
            return await _context.ScheduleActivities.Include(a => a.Schedule).FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<ScheduleActivity?> GetDeletedActivityByIdAsync(int id)
        {
            return await _context.ScheduleActivities.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == true);
        }
    }
}
