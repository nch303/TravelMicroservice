using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class ChatGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ScheduleId { get; set; }   // có thể null
        public Guid UserId { get; set; }        // người tạo group
        public ChatGroupType GroupType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
