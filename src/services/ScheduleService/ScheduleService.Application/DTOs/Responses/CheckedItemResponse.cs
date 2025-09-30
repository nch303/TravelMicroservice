using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class CheckedItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid ScheduleId { get; set; }
    }
}
