using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class ScheduleParticipant
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ParticipantRole Role { get; set; } 
        public DateTime JoineddAt { get; set; }
        public string Status { get; set; }

        // Foreign key to Schedule (1-N)
        public Guid ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
    }
}
