using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public MessageType MessageType { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EditAt { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Sent;

        // Navigation
        public Guid GroupId { get; set; }
        public ChatGroup Group { get; set; }

        public Guid SenderId { get; set; }
        public ChatParticipant Sender { get; set; }

        public Guid? ParentMessageId { get; set; }   // reply
        public ChatMessage ParentMessage { get; set; }

        public ICollection<ChatMessage> Replies { get; set; }
        public ICollection<MessageReaction> Reactions { get; set; }
        public ICollection<MessageRead> Reads { get; set; }
    }
}
