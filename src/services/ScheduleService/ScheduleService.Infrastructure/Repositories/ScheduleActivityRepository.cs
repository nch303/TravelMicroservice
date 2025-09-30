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