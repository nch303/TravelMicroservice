using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.DTOs.Requests
{
    public class CreateReaderRequest
    {
        public Guid GroupId { get; set; }
    }
}
