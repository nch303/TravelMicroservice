using AdvertisementService.Application.DTOs.Requests;
using AdvertisementService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertisementService.Application.IServiceClients
{
    public interface IUserServiceClient
    {
        Task CreateUserProfileAsync(CreateProfileRequest request);
        Task<ProfileResponse?> GetProfileAsync(Guid userId);
    }
}
