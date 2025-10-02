using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Responses
{
    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public MessageType MessageType { get; set; }
        public string MessageTypeText => ((MessageType)MessageType).ToString();
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditAt { get; set; }
        public MessageStatus Status { get; set; }
        public string StatusText => ((MessageStatus)Status).ToString();


        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public Guid? ParentMessageId { get; set; }   // reply
        //public ICollection<MessageReaction> Reactions { get; set; }
        //public ICollection<MessageRead> Reads { get; set; }
    }
}
