using Microsoft.AspNetCore.Http;
using ScheduleService.Application.DTOs.Responses;
using ScheduleService.Application.IServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.ServiceClients
{
    public class AuthServiceClient: IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AccountResponse?> GetCurrentAccountAsync()
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetAsync("api/auth/me");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get current account");
            }

            return await response.Content.ReadFromJsonAsync<AccountResponse>();
        }
    }
}
