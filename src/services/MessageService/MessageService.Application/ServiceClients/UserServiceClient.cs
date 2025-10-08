using MessageService.Application.DTOs.Responses;
using MessageService.Application.IServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.ServiceClients
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
