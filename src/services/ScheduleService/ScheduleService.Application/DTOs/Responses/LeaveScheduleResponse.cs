using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class LeaveScheduleResponse
    {
        public int ParticipantCounts { get; set; }
        public List<ScheduleParticipantResponse>? ScheduleParticipantResponses { get; set; }
    }
}
