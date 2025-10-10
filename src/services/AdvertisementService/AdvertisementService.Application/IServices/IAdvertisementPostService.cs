using AdvertisementService.Application.DTOs.Requests;
using AdvertisementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.IServices
{
    public interface IAdvertisementPostService
    {
        Task<AdvertisementPost> CreatePostAsync(Guid partnerId, CreateAdvertisementPostRequest advertisementPost);
        Task<IEnumerable<AdvertisementPost>> GetPostsByPartnerAsync(Guid partnerId);
        Task<AdvertisementPost?> GetPostByIdAsync(Guid id);
        Task<AdvertisementPost> UpdatePostAsync(Guid postId, UpdateAdvertisementPostRequest request);
    }
}
