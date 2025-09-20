using AuthService.Application.DTOs.Requests;
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
    }
}
