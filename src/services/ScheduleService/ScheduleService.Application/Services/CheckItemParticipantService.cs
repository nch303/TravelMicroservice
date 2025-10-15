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
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;

        public CheckItemParticipantService(ICheckItemParticipantRepository checkItemParticipantRepository, IAuthServiceClient authServiceClient, IScheduleParticipantRepository scheduleParticipantRepository)
        {
            _checkItemParticipantRepository = checkItemParticipantRepository;
            _authServiceClient = authServiceClient;
            _scheduleParticipantRepository = scheduleParticipantRepository;
        }

        public async Task ToggleCheckAsync(int checkedItemId, bool isChecked)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _scheduleParticipantRepository.GetParticipantByUserIdAsync(user!.Id);
            var success = await _checkItemParticipantRepository.ToggleCheckAsync(checkedItemId, participant!.Id, isChecked);
            if (!success)
                throw new KeyNotFoundException("CheckedItemParticipant not found");
        }

        //public async Task DeleteManyAsync(List<int> checkedItemId)
        //{
        //    await _checkItemParticipantRepository.DeleteManyAsync(checkedItemId);
        //}

        public async Task<List<CheckedItemParticipant>> GetByCurrentAccountAsync(Guid userId, Guid scheduleId)
        {
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(userId, scheduleId);
            if (participant == null)
                throw new Exception("No participant be found");
            var checkedList = await _checkItemParticipantRepository.GetByCurrentAccountAsync(participant!.Id);
            return checkedList;
        }
    }
}
