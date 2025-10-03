using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Domain.Enums;

namespace ScheduleService.Application.DTOs.Responses
{
    public class ScheduleMediaResponse
    {
        public int Id { get; set; }
        public MediaType MediaType { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedUserId { get; set; }
        public MediaMethod UploadMethod { get; set; }
        public Guid? ScheduleId { get; set; }
        public int? ActivityId { get; set; }
    }
}
