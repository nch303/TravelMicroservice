using AdvertisementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Domain.IRepositories
{
    public interface IAdvertisementPackageRepository
    {
        Task<IEnumerable<AdvertisementPackage>> GetAllAsync();
        Task<AdvertisementPackage?> GetByIdAsync(Guid id);
        Task AddAsync(AdvertisementPackage package);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
