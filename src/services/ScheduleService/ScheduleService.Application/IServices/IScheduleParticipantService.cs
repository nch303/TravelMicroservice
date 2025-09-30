using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;

namespace ScheduleService.Application.IServices
{
    public interface IScheduleParticipantService
    {
        Task<ScheduleParticipant> GetByUserIdAndScheduleIdAsync(Guid userId, Guid scheduleId);
        Task<List<ScheduleParticipant>> GetAllScheduleByParticipantIdAsync(Guid participantId);
        Task LeaveScheduleAsync(Guid scheduleId, Guid userId);
    }
}
