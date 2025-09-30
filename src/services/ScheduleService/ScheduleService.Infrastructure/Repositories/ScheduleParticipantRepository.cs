using Microsoft.EntityFrameworkCore;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;
using ScheduleService.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Infrastructure.Repositories
{
    public class ScheduleParticipantRepository: IScheduleParticipantRepository
    {
        private readonly AppDbContext _context;

        public ScheduleParticipantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleParticipant?> GetByUserIdAndScheduleIdAsync(Guid userId, Guid scheduleId)
        {
            return await _context.ScheduleParticipants
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.ScheduleId == scheduleId);
        }

        public async Task<ScheduleParticipant> AddScheduleParticipantAsync(ScheduleParticipant participant)
        {
            var entity = await _context.ScheduleParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScheduleParticipant>> GetAllScheduleByParticipantIdAsync(Guid participantId)
        {
            return await _context.ScheduleParticipants
                .Where(sp => sp.UserId == participantId)
                .ToListAsync();
        }

        public async Task<int> AmountParticipantsInScheduleAsync(Guid scheduleId)
        {
            return await _context.ScheduleParticipants
                .CountAsync(sp => sp.ScheduleId == scheduleId && sp.Status == "Active");
        }
    }
}
