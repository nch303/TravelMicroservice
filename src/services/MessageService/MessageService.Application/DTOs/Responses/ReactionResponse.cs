using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Responses
{
    public class ReactionResponse
    {
        public Guid Id { get; set; }
        public ReactionType ReactionType { get; set; }
        public string ReactionTypeText => ((ReactionType)ReactionType).ToString();
        public DateTime CreatedAt { get; set; }
        public Guid ParticipantId { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantAvatar { get; set; }
        public Guid MessageId { get; set; }
    }
}
