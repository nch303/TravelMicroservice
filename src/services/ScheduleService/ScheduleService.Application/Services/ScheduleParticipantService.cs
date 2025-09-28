using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Services
{
    public class ScheduleParticipantService: IScheduleParticipantService
    {
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;

        public ScheduleParticipantService(IScheduleParticipantRepository scheduleParticipantRepository)
        {
            _scheduleParticipantRepository = scheduleParticipantRepository;
        }

        public async Task<ScheduleParticipant?> GetByUserIdAndScheduleIdAsync(Guid userId, Guid scheduleId)
        {
            var schedule = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(userId, scheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule participant not found");
            }
            return schedule;
        }

        public async Task<List<ScheduleParticipant>> GetAllScheduleByParticipantIdAsync(Guid participantId)
        {
            var schedules = await _scheduleParticipantRepository.GetAllScheduleByParticipantIdAsync(participantId);
            if (schedules == null || !schedules.Any())
            {
                throw new Exception("No schedules found for this participant");
            }
            return schedules;
        }
    }
}
