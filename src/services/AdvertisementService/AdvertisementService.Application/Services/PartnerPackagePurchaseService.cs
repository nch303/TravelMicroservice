using AdvertisementService.Application.IServices;
using AdvertisementService.Domain.Entities;
using AdvertisementService.Domain.Enums;
using AdvertisementService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.Services
{
    public class PartnerPackagePurchaseService : IPartnerPackagePurchaseService
    {
        private readonly IPartnerPackagePurchaseRepository _purchaseRepository;
        private readonly IAdvertisementPackageRepository _packageRepository;

        public PartnerPackagePurchaseService(
            IPartnerPackagePurchaseRepository purchaseRepository,
            IAdvertisementPackageRepository packageRepository)
        {
            _purchaseRepository = purchaseRepository;
            _packageRepository = packageRepository;
        }

        public async Task<IEnumerable<PartnerPackagePurchase>> GetPurchasesByPartnerAsync(Guid partnerId)
        {
            var purchases = await _purchaseRepository.GetByPartnerIdAsync(partnerId);
            if(purchases == null || !purchases.Any())
                throw new Exception("Đối tác chưa mua gói quảng cáo nào.");

            return purchases;
        }

        public async Task<PartnerPackagePurchase?> GetByIdAsync(Guid id)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
                throw new Exception("Giao dịch mua gói quảng cáo không tồn tại.");

            return purchase;

        }

        public async Task<PartnerPackagePurchase> CreatePurchaseAsync(Guid partnerId, Guid packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null || !package.IsActive)
                throw new Exception("Gói quảng cáo không tồn tại hoặc không khả dụng.");

            var purchase = new PartnerPackagePurchase
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                PackageId = packageId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(package.DurationInDays),
                RemainingPostCount = package.MaxPostCount,
                Status = PartnerPackagePurchaseStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _purchaseRepository.AddAsync(purchase);
            await _purchaseRepository.SaveChangesAsync();

            return purchase;
        }
    }

}
