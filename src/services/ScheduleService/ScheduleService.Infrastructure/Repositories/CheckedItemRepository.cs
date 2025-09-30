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
    public class CheckedItemRepository : ICheckedItemRepository
    {
        private readonly AppDbContext _context;
        public CheckedItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCheckedItemsAsync(List<CheckedItem> items)
        {
            await _context.CheckedItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<CheckedItem>> GetByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.CheckedItems
                .Where(ci => ci.ScheduleId == scheduleId)
                .ToListAsync();
        }
    }
}
