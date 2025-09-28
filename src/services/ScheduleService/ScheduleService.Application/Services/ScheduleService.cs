using ScheduleService.Application.IServices;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Services
{
    public class SchedulesService: IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;

        public SchedulesService(IScheduleRepository scheduleRepository
            , IScheduleParticipantRepository scheduleParticipantRepository)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleParticipantRepository = scheduleParticipantRepository;
        }

        public async Task<Schedule> GetScheduleByIdAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            return schedule;
        }

        public async Task SaveChangesAsync()
        {
            await _scheduleRepository.SaveChangesAsync();
        }

        public async Task<string> ShareScheduleAsync(Guid id)
        {
            var schedule = await GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }

            // Logic to generate a share code
            var sharedCode = schedule.GenerateRandomCode(); 
            schedule.SharedCode = sharedCode;
            await SaveChangesAsync();
            return sharedCode;
        }

        public async Task JoinScheduleAsync(string sharedCode, Guid userId)
        {
            var schedule = await _scheduleRepository.GetScheduleByShareCodeAsync(sharedCode);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            
            var existingParticipant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(userId, schedule.Id);
            if (existingParticipant != null)
            {
                throw new Exception("User is already a participant in this schedule");
            }

            var amountOfParticipants = await _scheduleParticipantRepository.AmountParticipantsInScheduleAsync(schedule.Id);
            if (amountOfParticipants >= schedule.ParticipantsCount)
            {
                throw new Exception("Schedule has reached the maximum number of participants");
            }

            var participant = new ScheduleParticipant
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ScheduleId = schedule.Id,
                Role = ParticipantRole.Viewer,
                JoineddAt = DateTime.UtcNow,
                Status = "Active"
            };

            await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);
            await _scheduleRepository.SaveChangesAsync();
        }
    }
}
