using ScheduleService.Application.IServiceClients;
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
    public class SchedulesService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        private readonly IAuthServiceClient _authServiceClient;

        public SchedulesService(IScheduleRepository scheduleRepository
            , IScheduleParticipantRepository scheduleParticipantRepository
            , IAuthServiceClient authServiceClient)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _authServiceClient = authServiceClient;
        }

        private static void ValidateSchedule(Schedule validateSchedule, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(validateSchedule.Title))
                throw new ArgumentException("Title is required");
            if (validateSchedule.Title.Length > 50)
                throw new ArgumentException("Title must be at most 255 characters");

            if (!string.IsNullOrEmpty(validateSchedule.StartLocation) && validateSchedule.StartLocation.Length > 255)
                throw new ArgumentException("StartLocation must be at most 255 characters");

            if (!string.IsNullOrEmpty(validateSchedule.Destination) && validateSchedule.Destination.Length > 255)
                throw new ArgumentException("Destination must be at most 255 characters");

            if (validateSchedule.EndDate != default && validateSchedule.StartDate != default && validateSchedule.EndDate < validateSchedule.StartDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate");

            // Only validate ParticipantsCount if it's explicitly provided or we're creating
            if (!isUpdate || validateSchedule.ParticipantsCount != default)
            {
                if (validateSchedule.ParticipantsCount < 0)
                    throw new ArgumentException("ParticipantsCount must be non-negative");
            }
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
                if (existingParticipant.Status == ParticipantStatus.Banned)
                {
                    throw new Exception("You have been banned from this schedule. To re-join, contact to the onwer to be restored.");
                }
                else if (existingParticipant.Status == ParticipantStatus.Active)
                {
                    throw new Exception("You are already a participant in this schedule");
                }
                else if (existingParticipant.Status == ParticipantStatus.Left)
                {
                    existingParticipant.Status = ParticipantStatus.Active;
                    existingParticipant.JoineddAt = DateTime.UtcNow;
                    await _scheduleParticipantRepository.SaveChangesAsync();

                    // Increase participant count
                    schedule.ParticipantsCount += 1;
                    await _scheduleRepository.SaveChangesAsync();

                    return;
                }
            }

            var participant = new ScheduleParticipant
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ScheduleId = schedule.Id,
                Role = ParticipantRole.Viewer,
                JoineddAt = DateTime.UtcNow,
                Status = ParticipantStatus.Active
            };

            // Increase participant count
            schedule.ParticipantsCount += 1;

            await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);
            await _scheduleRepository.SaveChangesAsync();
        }

        public async Task<Schedule> UpdateScheduleByIdAsync(Schedule newSchedule, Guid id)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, id);
            if (participant!.Role != ParticipantRole.Owner && participant.Role != ParticipantRole.Editor)
            {
                throw new Exception("You do not have permission to update this schedule");
            }

            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with Id {id} not found.");
            }

            // Validate the new data first
            ValidateSchedule(newSchedule, isUpdate: true);

            // Update fields
            schedule.Title = newSchedule.Title;
            schedule.StartLocation = newSchedule.StartLocation;
            schedule.Destination = newSchedule.Destination;
            schedule.StartDate = newSchedule.StartDate;
            schedule.EndDate = newSchedule.EndDate;
            schedule.Notes = newSchedule.Notes;
            schedule.IsShared = newSchedule.IsShared;

            // Always update UpdatedAt
            schedule.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _scheduleRepository.SaveChangesAsync();

            return schedule;
        }

        public async Task<bool> CancelScheduleAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with Id {id} not found.");
            }
            schedule.Status = ScheduleStatus.Inactive;
            // Save changes
            await _scheduleRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestoreScheduleAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with Id {id} not found.");
            }
            schedule.Status = ScheduleStatus.Active;
            // Save changes
            await _scheduleRepository.SaveChangesAsync();

            return true;
        }

        public async Task CreateScheduleAsync(Schedule schedule)
        {
            ValidateSchedule(schedule, isUpdate: false);
            await _scheduleRepository.CreateScheduleAsync(schedule);
            await _scheduleRepository.SaveChangesAsync();
        }
    }
}
