using ScheduleService.Application.DTOs.Requests;
using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.IServices
{
    public interface IActivityAttendanceService
    {
        public Task<ActivityAttendance> CheckInAsync(Guid userId, AttendanceRequest attendanceRequest);
        public Task<ActivityAttendance> CheckOutAsync(Guid userId, AttendanceRequest attendanceRequest);

    }
}
