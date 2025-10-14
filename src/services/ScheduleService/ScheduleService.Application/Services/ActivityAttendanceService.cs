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
    public class ActivityAttendanceService : IActivityAttendanceService
    {
        private readonly IScheduleActivityRepository _scheduleActivityRepository;
        private readonly IScheduleParticipantRepository _participantRepository;
        private readonly IActivityAttendanceRepository _attendanceRepository;

        public ActivityAttendanceService(IScheduleActivityRepository scheduleActivityRepository, IScheduleParticipantRepository participantRepository
            , IActivityAttendanceRepository activityAttendanceRepository)
        {
            _scheduleActivityRepository = scheduleActivityRepository;
            _participantRepository = participantRepository;
            _attendanceRepository = activityAttendanceRepository;
        }

        public async Task CheckInAsync(int activityId, Guid userId)
        {
            // Validate participant
            var participant = await _participantRepository.GetParticipantByUserIdAsync(userId);
            if (participant == null)
            {
                throw new Exception("No participant was found");
            }

            // Validate activity
            var activity = await _scheduleActivityRepository.GetActivityByIdAsync(activityId);
            if (activity == null)
            {
                throw new Exception("No activity was found");
            }

            // Validate participant in activity
            var participants = await _participantRepository.GetAllParticipantByScheduleIdAsync(activity.ScheduleId);
            if (participants == null)
            {
                throw new Exception("No participant is in the activity");
            }

            if (participants.FirstOrDefault(p => p.Id == participant.Id) == null)
            {
                throw new Exception("The participant is not in this activity");
            }

            var attendance = new ActivityAttendance
            {
                Id = Guid.NewGuid(),
                CheckInTime = DateTime.UtcNow,
                CheckOutTime = null,
                Status = AttendanceStatus.CheckIn,
                ActivityId = activityId,
                ParticipantId = participant.Id
            };

            await _attendanceRepository.CreateAttendanceAsync(attendance);
        }

        public async Task CheckOutAsync(int activityId, Guid userId)
        {
            // Validate participant
            var participant = await _participantRepository.GetParticipantByUserIdAsync(userId);
            if (participant == null)
            {
                throw new Exception("No participant was found");
            }

            // Validate activity
            var activity = await _scheduleActivityRepository.GetActivityByIdAsync(activityId);
            if (activity == null)
            {
                throw new Exception("No activity was found");
            }

            // Validate participant in activity
            var participants = await _participantRepository.GetAllParticipantByScheduleIdAsync(activity.ScheduleId);
            if (participants == null)
            {
                throw new Exception("No participant is in the activity");
            }
            if (participants.FirstOrDefault(p => p.Id == participant.Id) == null)
            {
                throw new Exception("The participant is not in this activity");
            }

            // Validate check-in exists
            var existingAttendance = await _attendanceRepository.GetAttendanceByActivityAndParticipantAsync(activityId, participant.Id);
            if (existingAttendance == null || existingAttendance.Status != AttendanceStatus.CheckIn)
            {
                throw new Exception("You need to complete the check-in before check-out");
            }
            var attendance = new ActivityAttendance
            {
                Id = Guid.NewGuid(),
                CheckInTime = null,
                CheckOutTime = DateTime.UtcNow,
                Status = AttendanceStatus.CheckOut,
                ActivityId = activityId,
                ParticipantId = participant.Id
            };
            await _attendanceRepository.CreateAttendanceAsync(attendance);
        }
    }
}
