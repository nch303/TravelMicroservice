using AdminService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.DTOs.Responses
{
    public class AdvertisementPostResponse
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string StatusDisplay => Status.ToString();
        public DateTime CreatedAt { get; set; }
        public Guid PackagePurchaseId { get; set; }
        public List<Guid> MediaIds { get; set; } = new List<Guid>();
        public List<string> MediaUrls { get; set; } = new List<string>();
    }
}
