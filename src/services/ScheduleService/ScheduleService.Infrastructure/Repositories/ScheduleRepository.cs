using Microsoft.EntityFrameworkCore;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Infrastructure.Repositories
{
    public class ScheduleRepository: IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule?> GetScheduleByIdAsync(Guid id)
        {
            return await _context.Schedules.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Schedule?> GetScheduleByShareCodeAsync(string shareCode)
        {
            return await _context.Schedules
                .Include(s => s.ScheduleParticipants)
                .FirstOrDefaultAsync(s => s.SharedCode == shareCode);
        }
    }
}
