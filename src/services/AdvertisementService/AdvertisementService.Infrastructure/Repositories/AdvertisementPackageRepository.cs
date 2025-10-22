using AdvertisementService.Domain.Entities;
using AdvertisementService.Domain.IRepositories;
using AdvertisementService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Infrastructure.Repositories
{
    public class AdvertisementPackageRepository : IAdvertisementPackageRepository
    {
        private readonly AppDbContext _context;

        public AdvertisementPackageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdvertisementPackage>> GetAllActiveAsync()
        {
            return await _context.AdvertisementPackages.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<AdvertisementPackage>> GetAllAsync()
        {
            return await _context.AdvertisementPackages.ToListAsync();
        }

        public async Task<AdvertisementPackage?> GetByIdAsync(Guid id)
        {
            return await _context.AdvertisementPackages.FindAsync(id);
        }

        public async Task AddAsync(AdvertisementPackage package)
        {
            await _context.AdvertisementPackages.AddAsync(package);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.AdvertisementPackages.FindAsync(id);
            if (entity != null)
                entity.IsActive = false;
            await _context.SaveChangesAsync();

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
