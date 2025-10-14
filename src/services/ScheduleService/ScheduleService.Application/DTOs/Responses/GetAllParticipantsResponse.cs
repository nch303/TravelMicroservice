using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class GetAllParticipantsResponse
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; } 
        public string Role { get; set; }
        public string Status { get; set; }
        public string AvatarUrl { get; set; }
    }
}
