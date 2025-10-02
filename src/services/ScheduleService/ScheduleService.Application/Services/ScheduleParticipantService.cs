using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;

namespace ScheduleService.Application.Services
{
    public class ScheduleParticipantService: IScheduleParticipantService
    {
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        private readonly IScheduleRepository _scheduleRepository; 

        public ScheduleParticipantService(IScheduleParticipantRepository scheduleParticipantRepository, IScheduleRepository scheduleRepository)
        {
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _scheduleRepository = scheduleRepository;
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

        public async Task<Schedule?> LeaveScheduleAsync(Guid scheduleId, Guid userId)
        {
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(userId, scheduleId);
            if (participant == null || participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("User is not an active participant.");

            participant.Status = ParticipantStatus.Left;
            participant.JoineddAt = DateTime.UtcNow;

            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            if (schedule.ParticipantsCount > 0)
                schedule.ParticipantsCount--;

            schedule.UpdatedAt = DateTime.UtcNow;

            await _scheduleRepository.SaveChangesAsync();
            await _scheduleParticipantRepository.SaveChangesAsync();

            // Reload the schedule with participants included
            var updatedSchedule = await _scheduleRepository.GetScheduleWithParticipantsByIdAsync(scheduleId);

            return updatedSchedule;
        }


        public async Task<ScheduleParticipant> AddScheduleParticipantAsync(ScheduleParticipant participant)
        {
            var newParticipant = await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);
            await _scheduleParticipantRepository.SaveChangesAsync();
            return newParticipant;
        }
    }
}
