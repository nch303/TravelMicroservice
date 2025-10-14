using ScheduleService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid? ScheduleId { get; set; }

        public Guid SenderId { get; set; }      // 👉 Luôn là UserId
        public Guid? RecipientId { get; set; }    // 👉 Luôn là UserId

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation relationships 1-n vo Schedule
        public Schedule? Schedule { get; set; }

        // Navigation relationships 1-n vo NotificationRecipient
        public ICollection<NotificationRecipient> NotificationRecipients { get; set; }
            = new List<NotificationRecipient>();
    }
}
