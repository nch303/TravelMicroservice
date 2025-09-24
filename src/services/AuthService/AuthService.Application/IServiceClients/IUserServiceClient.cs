using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.IServiceClients
{
    public interface IUserServiceClient
    {
        Task CreateUserProfileAsync(CreateProfileRequest request);
        Task<ProfileResponse?> GetProfileAsync(Guid userId);
    }
}
