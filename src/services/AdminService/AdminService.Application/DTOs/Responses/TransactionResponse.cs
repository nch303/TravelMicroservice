using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.DTOs.Responses
{
    public class TransactionResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Gateway { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? AccountNumber { get; set; }
        public string? SubAccount { get; set; }
        public decimal AmountIn { get; set; }
        public decimal AmountOut { get; set; }
        public decimal Accumulated { get; set; }
        public string? Code { get; set; }
        public string? TransactionContent { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Thêm các khóa liên kết
        public Guid? PackageId { get; set; }      // gói quảng cáo được mua
        public Guid? UserId { get; set; }         // người thực hiện giao dịch
        public string Status { get; set; }        // Pending, Success, Failed, Verified...
    }
}
