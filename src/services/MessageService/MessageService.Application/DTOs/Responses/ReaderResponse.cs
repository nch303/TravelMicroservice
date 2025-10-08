using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Responses
{
    public class ReaderResponse
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid ReaderId { get; set; }
        public string ReaderName { get; set; }
        public string ReaderAvatar { get; set; }
        public DateTime ReadAt { get; set; }
    }
}
