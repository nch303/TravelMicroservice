using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageService.Domain.Enums;

namespace MessageService.Application.DTOs.Requests
{
    public class CreateReactionRequest
    {
        public Guid MessageId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
