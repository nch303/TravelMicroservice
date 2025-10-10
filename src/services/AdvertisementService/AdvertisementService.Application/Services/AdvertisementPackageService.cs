using AdvertisementService.Application.IServices;
using AdvertisementService.Domain.Entities;
using AdvertisementService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.Services
{
    public class AdvertisementPackageService : IAdvertisementPackageService
    {
        private readonly IAdvertisementPackageRepository _packageRepository;

        public AdvertisementPackageService(IAdvertisementPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<IEnumerable<AdvertisementPackage>> GetAllAsync()
            => await _packageRepository.GetAllAsync();

        public async Task<AdvertisementPackage?> GetByIdAsync(Guid id)
            => await _packageRepository.GetByIdAsync(id);

        public async Task<AdvertisementPackage> CreateAsync(AdvertisementPackage package)
        {
            package.CreatedAt = DateTime.UtcNow;
            package.IsActive = true;
            await _packageRepository.AddAsync(package);
            await _packageRepository.SaveChangesAsync();
            return package;
        }

        public async Task<AdvertisementPackage?> UpdateAsync(Guid id, AdvertisementPackage updated)
        {
            var existing = await _packageRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Price = updated.Price;
            existing.MaxPostCount = updated.MaxPostCount;
            existing.DurationInDays = updated.DurationInDays;

            await _packageRepository.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _packageRepository.DeleteAsync(id);
            return true;
        }
    }

}
