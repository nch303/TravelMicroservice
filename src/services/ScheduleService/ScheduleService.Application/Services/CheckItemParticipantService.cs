using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;

namespace ScheduleService.Application.Services
{
    public class CheckItemParticipantService : ICheckItemParticipantService
    {
        private readonly ICheckItemParticipantRepository _checkItemParticipantRepository;

        public CheckItemParticipantService(ICheckItemParticipantRepository checkItemParticipantRepository)
        {
            _checkItemParticipantRepository = checkItemParticipantRepository;
        }

        public async Task<CheckedItemParticipant> UpdateAsync(CheckedItemParticipant entity)
        {
            var updated = await _checkItemParticipantRepository.UpdateAsync(entity);
            if (updated == null)
                throw new KeyNotFoundException("CheckedItemParticipant not found");

            return updated;
        }

        public async Task ToggleCheckAsync(int checkedItemId, Guid scheduleParticipantId, bool isChecked)
        {
            var success = await _checkItemParticipantRepository.ToggleCheckAsync(checkedItemId, scheduleParticipantId, isChecked);
            if (!success)
                throw new KeyNotFoundException("CheckedItemParticipant not found");
        }

        public async Task DeleteManyAsync(List<(int checkedItemId, Guid scheduleParticipantId)> keys)
        {
            await _checkItemParticipantRepository.DeleteManyAsync(keys);
        }
    }
}
