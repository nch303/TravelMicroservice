using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Requests
{
    public class CreateChatGroupRequest
    {
        public string Name { get; set; }
        public Guid? ScheduleId { get; set; }   // có thể null
        public ChatGroupType GroupType { get; set; }
    }
}
