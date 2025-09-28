using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.IRepositories
{
    public interface IScheduleParticipantRepository
    {
        Task<ScheduleParticipant?> GetByUserIdAndScheduleIdAsync(Guid userId, Guid scheduleId);
        Task SaveChangesAsync();
        Task<ScheduleParticipant> AddScheduleParticipantAsync(ScheduleParticipant participant);
        Task<List<ScheduleParticipant>> GetAllScheduleByParticipantIdAsync(Guid participantId);
        Task<int> AmountParticipantsInScheduleAsync(Guid scheduleId);
    }
}
