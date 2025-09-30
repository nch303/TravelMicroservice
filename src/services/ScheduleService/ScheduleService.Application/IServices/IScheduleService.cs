using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.IServices
{
    public interface IScheduleService
    {
        Task<Schedule> GetScheduleByIdAsync(Guid id);
        Task SaveChangesAsync();
        Task<string> ShareScheduleAsync(Guid id);
        Task JoinScheduleAsync(string sharedCode, Guid userId);
        Task<Schedule> UpdateScheduleByIdAsync(Schedule newSchedule, Guid id);
        Task<bool> CancelScheduleAsync(Guid id);
        Task CreateScheduleAsync(Schedule schedule);
    }
}
