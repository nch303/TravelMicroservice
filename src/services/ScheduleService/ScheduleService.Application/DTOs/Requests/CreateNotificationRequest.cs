using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Requests
{
    public class CreateNotificationRequest
    {
        public Guid? ScheduleId { get; set; }
        public Guid? RecipientId { get; set; }    // 👉 Luôn là UserId

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
