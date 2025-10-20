using Microsoft.AspNetCore.Http;
using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Requests
{
    public class AttendanceRequest
    {
        public int ActivityId { get; set; }
        public IFormFile File { get; set; }
        public string? Description { get; set; }
    }
}