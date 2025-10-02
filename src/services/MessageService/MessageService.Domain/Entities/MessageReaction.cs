using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class MessageReaction
    {
        public Guid Id { get; set; }
   
        public string ReactionType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Guid UserId { get; set; }
        public ChatParticipant User { get; set; }

        public Guid MessageId { get; set; }
        public ChatMessage Message { get; set; }
    }
}
