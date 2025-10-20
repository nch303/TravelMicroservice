using AdvertisementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Domain.IRepositories
{
    public interface IAdvertisementPostRepository
    {
        Task<AdvertisementPost> AddAsync(AdvertisementPost post);
        Task<AdvertisementPost?> GetByIdAsync(Guid id);
        Task<List<AdvertisementPost>?> GetByPartnerIdAsync(Guid partnerId);
        Task SaveChangesAsync();
        Task<List<AdvertisementPost>?> GetAllPostAsync();
        Task<List<AdvertisementPost>?> GetApprovedPostAsync();
    }
}
