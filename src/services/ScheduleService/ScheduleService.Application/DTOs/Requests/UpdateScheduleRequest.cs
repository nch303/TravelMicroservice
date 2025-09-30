using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Requests
{
    public class UpdateScheduleRequest
    {
        public string? Title { get; set; }
        public string? StartLocation { get; set; }
        public string? Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
        public bool IsShared { get; set; }
    }
}
