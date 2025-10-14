using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.IServices
{
    public interface IActivityAttendanceService
    {
        public Task CheckInAsync(int activityId, Guid userId);
        public Task CheckOutAsync(int activityId, Guid userId);

    }
}
