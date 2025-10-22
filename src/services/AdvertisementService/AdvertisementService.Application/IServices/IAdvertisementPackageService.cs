using AdvertisementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.IServices
{
    public interface IAdvertisementPackageService
    {
        Task<IEnumerable<AdvertisementPackage>> GetAllActiveAsync();
        Task<IEnumerable<AdvertisementPackage>> GetAllAsync();
        Task<AdvertisementPackage?> GetByIdAsync(Guid id);
        Task<AdvertisementPackage> CreateAsync(AdvertisementPackage package);
        Task<AdvertisementPackage?> UpdateAsync(Guid id, AdvertisementPackage updated);
        Task<bool> DeleteAsync(Guid id);
        Task<AdvertisementPackage?> RestoreAsync(Guid id);
    }
}
