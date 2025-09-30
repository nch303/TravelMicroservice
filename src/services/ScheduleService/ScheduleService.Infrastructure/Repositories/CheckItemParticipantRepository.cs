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
    public class CheckItemParticipantRepository : ICheckItemParticipantRepository
    {
        private readonly AppDbContext _context;

        public CheckItemParticipantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CheckedItemParticipant?> GetByIdAsync(int checkedItemId, Guid scheduleParticipantId)
        {
            return await _context.CheckedItemParticipants
                .FirstOrDefaultAsync(c => c.CheckedItemId == checkedItemId
                                       && c.ScheduleParticipantId == scheduleParticipantId);
        }

        public async Task<CheckedItemParticipant?> UpdateAsync(CheckedItemParticipant entity)
        {
            var existing = await GetByIdAsync(entity.CheckedItemId, entity.ScheduleParticipantId);
            if (existing == null) return null;

            // update values (including ability to change CheckedItemId!)
            existing.CheckedItemId = entity.CheckedItemId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> ToggleCheckAsync(int checkedItemId, Guid scheduleParticipantId, bool isChecked)
        {
            var existing = await GetByIdAsync(checkedItemId, scheduleParticipantId);
            if (existing == null) return false;

            existing.IsChecked = isChecked;
            existing.CheckedAt = isChecked ? DateTime.UtcNow : default;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteManyAsync(List<(int checkedItemId, Guid scheduleParticipantId)> keys)
        {
            var items = await _context.CheckedItemParticipants
                .Where(c => keys.Contains(new ValueTuple<int, Guid>(c.CheckedItemId, c.ScheduleParticipantId)))
                .ToListAsync();

            _context.CheckedItemParticipants.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
