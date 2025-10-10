using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Application.IServiceClients;
using ScheduleService.Application.IServices;
using ScheduleService.Application.ServiceClients;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.IRepositories;
using Sprache;

namespace ScheduleService.Application.Services
{
    public class ScheduleParticipantService: IScheduleParticipantService
    {
        private readonly IScheduleParticipantRepository _scheduleParticipantRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IAuthServiceClient _authServiceClient;

        public ScheduleParticipantService(IScheduleParticipantRepository scheduleParticipantRepository, IScheduleRepository scheduleRepository, IUserServiceClient userServiceClient, IAuthServiceClient authServiceClient)
        {
            _scheduleParticipantRepository = scheduleParticipantRepository;
            _scheduleRepository = scheduleRepository;
            _userServiceClient = userServiceClient;
            _authServiceClient = authServiceClient;
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
            return schedules ?? new List<ScheduleParticipant>();
        }

        public async Task<Schedule?> LeaveScheduleAsync(Guid scheduleId)
        {
            var user = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(user!.Id, scheduleId);
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

        public async Task ChangeParticipantRoleAsync(Guid participantId, Guid scheduleId)
        {
            // Get current user (the one making the request)
            var currentUser = await _authServiceClient.GetCurrentAccountAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            // Get the schedule
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            // Ensure current user is the owner
            if (schedule.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("Only the schedule owner can change participant's role.");

            // Get the participant to change role
            var participant = await _scheduleParticipantRepository.GetParticipantByIdAsync(participantId);
            if (participant == null)
                throw new KeyNotFoundException("Participant not found.");
            if (participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("Participant is not currently active.");

            // Mark participant as 'Left'
            participant.Role = ParticipantRole.Editor;
            schedule.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _scheduleParticipantRepository.SaveChangesAsync();
        }

        public async Task<Schedule?> KickParticipantAsync(Guid scheduleId, Guid participantId)
        {
            // Get current user (the one making the request)
            var currentUser = await _authServiceClient.GetCurrentAccountAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            // Get the schedule
            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            // Ensure current user is the owner
            if (schedule.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("Only the schedule owner can kick participants.");

            // Get the participant to remove
            var participant = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(participantId, scheduleId);
            if (participant == null)
                throw new KeyNotFoundException("Participant not found.");
            if (participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("Participant is not currently active.");

            // Owner cannot kick themselves
            if (participant.UserId == currentUser.Id)
                throw new InvalidOperationException("Owner cannot kick themselves from the schedule.");

            // Mark participant as 'Left'
            participant.Status = ParticipantStatus.Banned;

            // Decrease participant count safely
            if (schedule.ParticipantsCount > 0)
                schedule.ParticipantsCount--;

            schedule.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _scheduleParticipantRepository.SaveChangesAsync();
            await _scheduleRepository.SaveChangesAsync();

            // Reload the updated schedule with participants
            var updatedSchedule = await _scheduleRepository.GetScheduleWithParticipantsByIdAsync(scheduleId);

            return updatedSchedule;
        }

        public async Task<ScheduleParticipant> AddScheduleParticipantAsync(ScheduleParticipant participant)
        {
            var newParticipant = await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);
            await _scheduleParticipantRepository.SaveChangesAsync();
            return newParticipant;
        }

        public async Task<ScheduleParticipant> AddParticipantByEmailAsync(Guid scheduleId, string email)
        {
            // 1) Ensure current user is owner of the schedule
            var currentUser = await _authServiceClient.GetCurrentAccountAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");

            var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            if (schedule.OwnerId != currentUser.Id)
                throw new UnauthorizedAccessException("User has no permission");

            // 2) Validate email exists and is active via AuthService
            var account = await _authServiceClient.GetAccountByEmailAsync(email);
            if (account == null || !account.IsActive)
                throw new Exception("email is invalid");

            // 3) Check duplication
            var existing = await _scheduleParticipantRepository.GetByUserIdAndScheduleIdAsync(account.Id, scheduleId);
            if (existing != null && existing.Status == ParticipantStatus.Active)
                throw new Exception("This user is already in the schedule");

            // 4) Create or reactivate participant
            if (existing != null && existing.Status != ParticipantStatus.Active)
            {
                existing.Status = ParticipantStatus.Active;
                existing.JoineddAt = DateTime.UtcNow;
                await _scheduleParticipantRepository.SaveChangesAsync();
                return existing;
            }

            var participant = new ScheduleParticipant
            {
                Id = Guid.NewGuid(),
                ScheduleId = scheduleId,
                UserId = account.Id,
                Role = ParticipantRole.Viewer,
                Status = ParticipantStatus.Active,
                JoineddAt = DateTime.UtcNow
            };

            var created = await _scheduleParticipantRepository.AddScheduleParticipantAsync(participant);

            // Update schedule participant count
            schedule.ParticipantsCount++;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _scheduleRepository.SaveChangesAsync();

            return created;
        }

        public async Task<(List<ScheduleParticipant> Participants, List<UserServiceClientResponse> Users)>
    GetAllParticipantByScheduleIdAsync(Guid scheduleId)
        {
            var participants = await _scheduleParticipantRepository.GetAllParticipantByScheduleIdAsync(scheduleId);

            if (participants == null || !participants.Any())
            {
                throw new KeyNotFoundException($"No participants found for ScheduleId: {scheduleId}");
            }

            var userIds = participants.Select(p => p.UserId).Distinct().ToList();
            var users = await _userServiceClient.GetUsersByIdsAsync(userIds);

            return (participants, users);
        }
    }
}
