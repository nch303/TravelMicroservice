using PaymentService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.DTOs.Responses
{
    public class PurchaseResponse
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingPostCount { get; set; }
        public Guid PaymentTransactionId { get; set; }
        public PartnerPackagePurchaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
    }
}
