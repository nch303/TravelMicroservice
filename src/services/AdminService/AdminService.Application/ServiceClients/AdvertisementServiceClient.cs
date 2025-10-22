using AdminService.Application.DTOs.Requests;
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
    public class AdvertisementServiceClient : IAdvertisementServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdvertisementServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<PackageResponse>?> GetAllPackagesAsync()
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetAsync("api/advertisement/admin/package/get-all");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get package");
            }

            return await response.Content.ReadFromJsonAsync<List<PackageResponse>>();
        }

        public async Task<PackageResponse?> GetPackageByIdAsync(Guid packageId)
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetAsync($"api/advertisement/package/get-by-id/{packageId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get package");
            }

            return await response.Content.ReadFromJsonAsync<PackageResponse>();
        }
        public async Task<PackageResponse?> CreatePackageAsync(CreatePackageRequest request)
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PostAsJsonAsync($"api/advertisement/package/create", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get package");
            }

            return await response.Content.ReadFromJsonAsync<PackageResponse>();
        }

        public async Task<PackageResponse?> UpdatePackageAsync(Guid packageId, UpdatePackageRequest request)
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PutAsJsonAsync($"api/advertisement/package/update/{packageId}", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update package");
            }

            return await response.Content.ReadFromJsonAsync<PackageResponse>();
        }

        public async Task<PackageResponse?>  DeletePackageAsync(Guid packageId)
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.DeleteAsync($"api/advertisement/package/delete/{packageId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete package");
            }

            return await response.Content.ReadFromJsonAsync<PackageResponse>();
        }

        public async Task<PackageResponse?> RestorePackageAsync(Guid packageId)
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.PatchAsJsonAsync($"api/advertisement/package/restore/{packageId}", packageId);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to restore package");
            }

            return await response.Content.ReadFromJsonAsync<PackageResponse>();
        }

        public async Task<List<AdvertisementPostResponse>?> GetAllPostAsync()
        {
            // Lấy token từ request hiện tại
            var accessToken = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var response = await _httpClient.GetAsync($"api/advertisement/post/all");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get post");
            }

            return await response.Content.ReadFromJsonAsync<List<AdvertisementPostResponse>>();
        }

    }
}
