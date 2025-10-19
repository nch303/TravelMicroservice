using AdminService.Application.DTOs.Responses;
using AdminService.Application.IServiceClients;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.ServiceClients
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TransactionResponse>?> GetAllTransactionsAsync()
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetAsync("api/payment/transaction/all");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get transactions");
            }

            return await response.Content.ReadFromJsonAsync<List<TransactionResponse>>();
        }
    }
}
