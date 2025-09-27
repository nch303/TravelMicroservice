using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Application.IServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.ServiceClients
{
    public class UserServiceClient: IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateUserProfileAsync(CreateProfileRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/user/create", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create user profile");
            }
        }

        public async Task<ProfileResponse?> GetProfileAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get profile for user {userId}");
            }

            return await response.Content.ReadFromJsonAsync<ProfileResponse>();
        }
    }
}
