using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Application.ServiceClients;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;

namespace ScheduleService.Application.Services
{
    public class ScheduleActivityService : IScheduleActivityService
    {
        private readonly IScheduleAcitvitiyRepository _scheduleActivityRepository;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        public ScheduleActivityService(IScheduleAcitvitiyRepository scheduleActivityRepository,IScheduleParticipantRepository scheduleParticipantRepository,
            IScheduleRepository scheduleRepository, IAuthServiceClient authServiceClient)
        {
            _scheduleActivityRepository = scheduleActivityRepository;
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _scheduleRepository = scheduleRepository;
            _authServiceClient = authServiceClient;
        }

        public async Task AddActivityAsync(ScheduleActivity activity)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(activity.ScheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, activity.ScheduleId);
            if (participant == null || (participant.Role != ParticipantRole.Owner && participant.Role != ParticipantRole.Editor))
            {
                throw new Exception("You do not have permission to add activity to this schedule");
            }
            await _scheduleActivityRepository.AddActivityAsync(activity);
        }

        public async Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
            if (participant == null)
            {
                throw new Exception("You do not have permission to view activities of this schedule");
            }

            var activities = await _scheduleActivityRepository.GetActivitiesByScheduleIdAsync(scheduleId);
            return activities;
        }
    }
}
