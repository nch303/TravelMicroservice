using AdminService.Application.DTOs.Requests;
using AdminService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.IServiceClients
{
    public interface IAdvertisementServiceClient
    {
        Task<List<PackageResponse>> GetAllPackagesAsync();
        Task<PackageResponse?> GetPackageByIdAsync(Guid id);
        Task<PackageResponse?> CreatePackageAsync(CreatePackageRequest request);
        Task<PackageResponse?> UpdatePackageAsync(Guid packageId, UpdatePackageRequest request);
        Task<PackageResponse?> DeletePackageAsync(Guid packageId);
        Task<PackageResponse?> RestorePackageAsync(Guid packageId);
        Task<List<AdvertisementPostResponse>?> GetAllPostAsync();
    }
}
