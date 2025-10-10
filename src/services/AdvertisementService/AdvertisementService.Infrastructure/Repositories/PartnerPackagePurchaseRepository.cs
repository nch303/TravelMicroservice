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
    public class PartnerPackagePurchaseRepository : IPartnerPackagePurchaseRepository
    {
        private readonly AppDbContext _context;

        public PartnerPackagePurchaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PartnerPackagePurchase>> GetByPartnerIdAsync(Guid partnerId)
        {
            return await _context.PartnerPackagePurchases
                .Include(p => p.Package)
                .Where(p => p.PartnerId == partnerId)
                .ToListAsync();
        }

        public async Task<PartnerPackagePurchase?> GetByIdAsync(Guid id)
        {
            return await _context.PartnerPackagePurchases
                .Include(p => p.Package)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(PartnerPackagePurchase purchase)
        {
            await _context.PartnerPackagePurchases.AddAsync(purchase);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
