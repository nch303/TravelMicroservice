using MessageService.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Requests
{
    public class EditMessageRequest
    {
        public MessageType MessageType { get; set; } 
        public string NewContent { get; set; }
        public IFormFile? File { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
