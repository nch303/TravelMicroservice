using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class UserServiceClientResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
