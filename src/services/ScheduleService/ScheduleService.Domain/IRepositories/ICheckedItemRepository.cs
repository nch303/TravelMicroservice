using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Domain.IRepositories
{
    public interface ICheckedItemRepository
    {
        Task AddCheckedItemsAsync(List<CheckedItem> items);
        Task SaveChangesAsync();
        Task<List<CheckedItem>> GetByScheduleIdAsync(Guid scheduleId);
    }
}
