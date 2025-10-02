using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;

namespace ScheduleService.Application.Services
{
    public class CheckItemParticipantService : ICheckItemParticipantService
    {
        private readonly ICheckItemParticipantRepository _checkItemParticipantRepository;
        private readonly IAuthServiceClient _authServiceClient;

        public CheckItemParticipantService(ICheckItemParticipantRepository checkItemParticipantRepository, IAuthServiceClient authServiceClient)
        {
            _checkItemParticipantRepository = checkItemParticipantRepository;
            _authServiceClient = authServiceClient;
        }

        public async Task ToggleCheckAsync(int checkedItemId, bool isChecked)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var success = await _checkItemParticipantRepository.ToggleCheckAsync(checkedItemId, user!.Id, isChecked);
            if (!success)
                throw new KeyNotFoundException("CheckedItemParticipant not found");
        }

        //public async Task DeleteManyAsync(List<int> checkedItemId)
        //{
        //    await _checkItemParticipantRepository.DeleteManyAsync(checkedItemId);
        //}
    }
}
