using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.IRepositories
{
    public interface IScheduleRepository
    {
        Task<Schedule?> GetScheduleByIdAsync(Guid id);
        Task SaveChangesAsync();
        Task<Schedule?> GetScheduleByShareCodeAsync(string shareCode);
        Task CreateScheduleAsync(Schedule schedule);
        Task<Schedule?> GetScheduleWithParticipantsByIdAsync(Guid scheduleId);
    }
}
