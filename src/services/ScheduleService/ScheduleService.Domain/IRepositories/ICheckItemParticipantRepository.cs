using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Domain.IRepositories
{
    public interface ICheckItemParticipantRepository
    {
        Task<CheckedItemParticipant?> GetByIdAsync(int checkedItemId, Guid scheduleParticipantId);
        Task<CheckedItemParticipant?> UpdateAsync(CheckedItemParticipant entity);
        Task<bool> ToggleCheckAsync(int checkedItemId, Guid scheduleParticipantId, bool isChecked);
        Task DeleteManyAsync(List<(int checkedItemId, Guid scheduleParticipantId)> keys);
    }
}
