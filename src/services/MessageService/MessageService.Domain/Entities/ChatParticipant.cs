using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class ChatParticipant
    {
        public Guid Id { get; set; }
        public Guid ParticipantId { get; set; }
        public ParticipantRole Role { get; set; }
        public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeenAt { get; set; }

        // Navigation
        public Guid ChatGroupId { get; set; }
        public ChatGroup ChatGroup { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<MessageReaction> Reactions { get; set; }
        public ICollection<MessageRead> Reads { get; set; }
    }
}
