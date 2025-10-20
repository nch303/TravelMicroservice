using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Application.IServiceClients;

namespace ScheduleService.Application.ServiceClients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserServiceClientResponse>> GetUsersByIdsAsync(List<Guid> userIds)
        {
            var response = await _httpClient.PostAsJsonAsync("api/user/batch", userIds);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<UserServiceClientResponse>>();
        }
    }

}
