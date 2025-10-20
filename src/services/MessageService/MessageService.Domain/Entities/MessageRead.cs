using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class MessageRead
    {
        public Guid Id { get; set; }
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Guid ReaderId { get; set; }
        public ChatParticipant Reader { get; set; }

        public Guid MessageId { get; set; }
        public ChatMessage Message { get; set; }
    }
}
