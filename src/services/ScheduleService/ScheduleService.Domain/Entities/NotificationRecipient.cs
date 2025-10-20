using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.Entities
{
    public class NotificationRecipient
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Guid RecipientId { get; set; }  // user nhận thông báo
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public Notification Notification { get; set; } = null!;
    }
}
