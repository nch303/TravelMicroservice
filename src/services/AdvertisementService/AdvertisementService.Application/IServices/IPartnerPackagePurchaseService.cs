using AdvertisementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.IServices
{
    public interface IPartnerPackagePurchaseService
    {
        Task<List<PartnerPackagePurchase>?> GetPurchasesByPartnerAsync(Guid partnerId);
        Task<PartnerPackagePurchase?> GetByIdAsync(Guid id);
        Task<PartnerPackagePurchase> CreatePurchaseAsync(Guid partnerId, Guid packageId, Guid transactionId);
    }

}
