using AdminService.Application.DTOs.Requests;
using AdminService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.IServiceClients
{
    public interface IUserServiceClient
    {
        Task CreateUserProfileAsync(CreateProfileRequest request);
        Task<ProfileResponse?> GetProfileAsync(Guid userId);
    }
}
