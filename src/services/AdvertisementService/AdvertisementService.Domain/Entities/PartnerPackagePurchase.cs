using AdvertisementService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Domain.Entities
{
    public class PartnerPackagePurchase
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingPostCount { get; set; }
        public Guid PaymentTransactionId { get; set; }
        public PartnerPackagePurchaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property to link to the AdvertisementPackage
        public Guid PackageId { get; set; }
        public AdvertisementPackage Package { get; set; }

        // Navigation property to link to the AdvertisementPosts made under this purchase
        public ICollection<AdvertisementPost> AdvertisementPosts { get; set; } = new List<AdvertisementPost>();
    }
}
