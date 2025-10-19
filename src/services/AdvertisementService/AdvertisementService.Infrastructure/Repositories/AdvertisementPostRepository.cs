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
    public class AdvertisementPostRepository : IAdvertisementPostRepository
    {
        private readonly AppDbContext _context;

        public AdvertisementPostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdvertisementPost> AddAsync(AdvertisementPost post)
        {
            await _context.AdvertisementPosts.AddAsync(post);
            return post;
        }

        public async Task<AdvertisementPost?> GetByIdAsync(Guid id)
        {
            return await _context.AdvertisementPosts
                .Include(p => p.MediaItems)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<AdvertisementPost>?> GetByPartnerIdAsync(Guid partnerId)
        {
            return await _context.AdvertisementPosts
                .Include(p => p.MediaItems)
                .Where(p => p.PartnerId == partnerId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdvertisementPost>?> GetAllPostAsync()
        {
            return await _context.AdvertisementPosts.Include(a => a.MediaItems).ToListAsync();
        }
    }
}
