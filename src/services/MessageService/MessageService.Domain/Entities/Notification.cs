using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }          // người nhận
        public Guid? ChatGroupId { get; set; }    // nhóm liên quan (nếu có)
        public Guid? ChatMessageId { get; set; }  // tin nhắn liên quan (nếu có)
        public string Title { get; set; }         // tiêu đề thông báo
        public string Content { get; set; }       // nội dung chi tiết
        public bool IsRead { get; set; } = false; // đã đọc hay chưa
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ChatGroup ChatGroup { get; set; }
        public ChatMessage ChatMessage { get; set; }
    }

}
