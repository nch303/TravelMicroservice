using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.DTOs.Responses
{
    public class NotificationRecipientResponse
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SenderId { get; set; }      // 👉 Luôn là UserId
        public string SenderName { get; set; }
        public Guid RecipientId { get; set; }  // user nhận thông báo
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; } = DateTime.UtcNow;

    }
}
