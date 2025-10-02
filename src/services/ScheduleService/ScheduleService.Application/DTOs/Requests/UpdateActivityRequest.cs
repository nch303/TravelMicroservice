using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Requests
{
    public class UpdateActivityRequest
    {
        public string? PlaceName { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
    }
}
