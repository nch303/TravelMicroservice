using ScheduleService.Application.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.IServices
{
    public interface IActivityAttendanceService
    {
        public Task CheckInAsync(Guid userId, AttendanceRequest attendanceRequest);
        public Task CheckOutAsync(Guid userId, AttendanceRequest attendanceRequest);

    }
}
