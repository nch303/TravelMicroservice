using PaymentService.Application.DTOs.Responses;
using PaymentService.Application.IServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.ServiceClients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProfileResponse?> GetUserProfileAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"api/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get profile for user {userId}");
            }

            return await response.Content.ReadFromJsonAsync<ProfileResponse>();
        }
    }
}
