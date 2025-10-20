using AdvertisementService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Domain.Entities
{
    public class AdvertisementPost
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public AdvertisementStatus Status { get; set; } // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }

        // Navigation property to link to the PartnerPackagePurchase
        public Guid PackagePurchaseId { get; set; }
        public PartnerPackagePurchase PackagePurchase { get; set; }

        // Navigation property to link to the AdvertisementMedia
        public ICollection<AdvertisementMedia> MediaItems { get; set; } = new List<AdvertisementMedia>();

    }
}
