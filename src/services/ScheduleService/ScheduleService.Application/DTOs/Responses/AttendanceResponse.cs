using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class AttendanceResponse
    {
        public Guid Id { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; }

        public int ActivityId { get; set; }

        public Guid ParticipantId { get; set; }
    }
}
