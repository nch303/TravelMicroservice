using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.IRepositories
{
    public interface IActivityAttendanceRepository
    {
        Task CreateAttendanceAsync(ActivityAttendance activityAttendance);
        Task SaveChangesAsync();
        Task<ActivityAttendance?> GetCheckInAttendanceByActivityAndParticipantAsync(int activityId, Guid participantId);
        Task<ActivityAttendance?> GetCheckOutAttendanceByActivityAndParticipantAsync(int activityId, Guid participantId);
    }
}
