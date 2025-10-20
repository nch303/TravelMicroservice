using MessageService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.Entities
{
    public class ChatGroup
    {
        public Guid Id { get; set; }
        public string? SharedCode { get; set; }  // mã nhóm, dùng để mời người khác vào nhóm
        public DateTime? SharedExpired { get; set; }
        public string Name { get; set; }
        public Guid? ScheduleId { get; set; }   // có thể null
        public Guid UserId { get; set; }        // người tạo group
        public ChatGroupType GroupType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string GenerateRandomCode(int hoursValid)
        {
            // Lấy Guid, chuyển sang số và giữ 10 ký tự số
            string digits = new string(Guid.NewGuid().ToString("N")
                                        .Where(char.IsDigit)
                                        .ToArray());
            SharedExpired = DateTime.UtcNow.AddHours(hoursValid);
            SharedCode = digits;
            return digits.Substring(0, 10);
            
        }

        public bool IsSharedCodeValid()
        {
            return SharedExpired == null || SharedExpired > DateTime.UtcNow;
        }

        // Navigation
        public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
