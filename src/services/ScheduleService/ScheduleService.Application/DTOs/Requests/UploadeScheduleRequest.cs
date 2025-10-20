using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ScheduleService.Domain.Enums;

namespace ScheduleService.Application.DTOs.Requests
{
    public class UploadScheduleMediaRequest
    {
        public IFormFile File { get; set; }
        public string? Description { get; set; }
        public MediaMethod UploadMethod { get; set; } // CheckIn | CheckOut | Normal

        public Guid? ScheduleId { get; set; }
        public int? ActivityId { get; set; }
    }
}
