using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Entities;

namespace ScheduleService.Application.DTOs.Responses
{
    public class ScheduleActivityResponse
    {
        public int Id { get; set; }
        public string PlaceName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int OrderIndex { get; set; }
        public bool IsDeleted { get; set; }

        // Foreign key to Schedule (1-N)
        public Guid ScheduleId { get; set; }
    }

}
