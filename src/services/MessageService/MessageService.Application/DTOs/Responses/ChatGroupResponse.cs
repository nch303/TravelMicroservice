using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Responses
{
    public class ChatGroupResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ScheduleId { get; set; } 
        public ChatGroupType GroupType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
