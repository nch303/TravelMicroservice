using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Enums;

namespace ScheduleService.Application.DTOs.Responses
{
    public class ScheduleParticipantResponse
    {
        public Guid UserId { get; set; }
        public ParticipantRole Role { get; set; }
        public DateTime JoineddAt { get; set; }
        public ParticipantStatus Status { get; set; }
    }
}
