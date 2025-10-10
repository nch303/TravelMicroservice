using AdvertisementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Domain.IRepositories
{
    public interface IPartnerPackagePurchaseRepository
    {
        Task<List<PartnerPackagePurchase>> GetByPartnerIdAsync(Guid partnerId);
        Task<PartnerPackagePurchase?> GetByIdAsync(Guid id);
        Task AddAsync(PartnerPackagePurchase purchase);
        Task SaveChangesAsync();
    }

}
