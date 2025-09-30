using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Domain.IRepositories
{
    public interface IScheduleAcitvitiyRepository
    {
        Task AddActivityAsync(ScheduleActivity activity);
        Task<List<ScheduleActivity>> GetActivitiesByScheduleIdAsync(Guid scheduleId);
    }
}
