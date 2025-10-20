using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class ActivityAttendance
    {
        public Guid Id { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public AttendanceStatus Status { get; set; }

        public int ActivityId { get; set; }
        public ScheduleActivity Activity { get; set; }

        public Guid ParticipantId { get; set; }
        public ScheduleParticipant Participant { get; set; }

    }
}
