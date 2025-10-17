using Microsoft.AspNetCore.Http;
using PaymentService.Application.DTOs.Requests;
using PaymentService.Application.DTOs.Responses;
using PaymentService.Application.IServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.ServiceClients
{
    public class AdvertisementServiceClient : IAdvertisementServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdvertisementServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PackageResponse?> GetPackageByIdAsync(Guid packageId)
        {
            var response = await _httpClient.GetAsync($"api/advertisement/package/get-by-id/{packageId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get package");
            }

            return await response.Content.ReadFromJsonAsync<PackageResponse>();
        }

        public async Task<PurchaseResponse?> CreatePurchaseAsync(CreatePurchaseRequest request)
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync("api/advertisement/partner/purchase/create", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create purchase");
            }

            return await response.Content.ReadFromJsonAsync<PurchaseResponse>();
        }
    }
}
