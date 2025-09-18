using System;

namespace UserService.Domain.Entities
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
    }
}
