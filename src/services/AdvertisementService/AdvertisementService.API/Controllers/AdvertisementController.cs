using AdvertisementService.Application.DTOs.Requests;
using AdvertisementService.Application.DTOs.Responses;
using AdvertisementService.Application.IServiceClients;
using AdvertisementService.Application.IServices;
using AdvertisementService.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisementService.API.Controllers
{
    [ApiController]
    [Route("api/advertisement")]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementPackageService _packageService;
        private readonly IMapper _mapper;
        private readonly IPartnerPackagePurchaseService _purchaseService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IAdvertisementPostService _advertisementService;
        private readonly ICloudinaryService _cloudinaryService;

        public AdvertisementController(IAdvertisementPackageService packageService, IMapper mapper
            , IPartnerPackagePurchaseService purchaseService, IUserServiceClient userServiceClient
            , IAuthServiceClient authServiceClient, IAdvertisementPostService advertisementService
            , ICloudinaryService cloudinaryService)
        {
            _packageService = packageService;
            _mapper = mapper;
            _purchaseService = purchaseService;
            _userServiceClient = userServiceClient;
            _authServiceClient = authServiceClient;
            _advertisementService = advertisementService;
            _cloudinaryService = cloudinaryService;
        }

        private DateTime ConvertToUtc7(DateTime localDateTime)
        {
            // Convert sang giờ VN (UTC+7)
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(localDateTime, vnTimeZone);
            return localTime;
        }


        [HttpGet("package/get-all")]
        [Authorize(Roles = "Partner,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _packageService.GetAllAsync();
            var packagesResponse = _mapper.Map<List<PackageResponse>>(packages);
            return Ok(packagesResponse);
        }

        [HttpGet("package/get-by-id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pkg = await _packageService.GetByIdAsync(id);
            var packageResponse = _mapper.Map<PackageResponse>(pkg);
            return Ok(packageResponse);
        }

        [HttpPost("package/create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePackageRequest request)
        {
            var package = _mapper.Map<AdvertisementPackage>(request);
            package.Id = Guid.NewGuid();
            package.IsActive = true;
            package.CreatedAt = DateTime.UtcNow;

            var created = await _packageService.CreateAsync(package);

            var packageResponse = _mapper.Map<PackageResponse>(created);
            packageResponse.CreatedAt = ConvertToUtc7(created.CreatedAt);

            return Ok(packageResponse);
        }

        [HttpPut("package/update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePackageRequest request)
        {
            var package = _mapper.Map<AdvertisementPackage>(request);
            var updated = await _packageService.UpdateAsync(id, package);
            var packageResponse = _mapper.Map<PackageResponse>(updated);
            return Ok(packageResponse);
        }

        [HttpDelete("package/delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _packageService.DeleteAsync(id);
            var package = await _packageService.GetByIdAsync(id);
            var response = _mapper.Map<PackageResponse>(package);
            return Ok(response);
        }

        [HttpPatch("package/restore/{id}")]
        [Authorize]
        public async Task<IActionResult> Restore(Guid id)
        {
            var package = await _packageService.RestoreAsync(id);
            var response = _mapper.Map<PackageResponse>(package);
            return Ok(response);
        }

        // Lấy danh sách gói đã mua của Partner
        [HttpGet("partner/purchase/get-by-partnerId/{partnerId}")]
        public async Task<IActionResult> GetPurchaseByPartner(Guid partnerId)
        {
            var purchases = await _purchaseService.GetPurchasesByPartnerAsync(partnerId);
            var purchasesResponse = _mapper.Map<List<PurchaseResponse>>(purchases);
            return Ok(purchasesResponse);
        }

        // Lấy chi tiết một gói đã mua
        [HttpGet("partner/purchase/get-by-id/{id}")]
        public async Task<IActionResult> GetByPurchaseById(Guid id)
        {
            var purchase = await _purchaseService.GetByIdAsync(id);
            if (purchase == null) return NotFound();

            var purchaseResponse = _mapper.Map<PurchaseResponse>(purchase);
            return Ok(purchaseResponse);
        }

        // Mua gói quảng cáo (chưa tích hợp thanh toán)
        [HttpPost("partner/purchase/create")]
        public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request)
        {
            var purchase = await _purchaseService.CreatePurchaseAsync(request.UserId, request.PackageId, request.TransactionId);
            var purchaseResponse = _mapper.Map<PurchaseResponse>(purchase);
            return Ok(purchaseResponse);
        }

        [HttpPost("partner/post/create")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] CreateAdvertisementPostRequest request)
        {
            var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
            // Gọi service xử lý toàn bộ logic (upload + lưu DB)
            var post = await _advertisementService.CreatePostAsync(currentAccount.Id, request);
            var postResponse = _mapper.Map<AdvertisementPostResponse>(post);
            return Ok(postResponse);
        }

        [HttpGet("partner/post/get-by-partnerId/{partnerId}")]
        public async Task<IActionResult> GetPostsByPartner(Guid partnerId)
        {
            var posts = await _advertisementService.GetPostsByPartnerAsync(partnerId);
            var postsResponse = _mapper.Map<List<AdvertisementPostResponse>>(posts);
            return Ok(postsResponse);
        }

        [HttpGet("post/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPostAsync()
        {
            try
            {
                var posts = await _advertisementService.GetAllPostAsync();
                var reponses = _mapper.Map<List<AdvertisementPostResponse>>(posts);
                return Ok(reponses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("partner/post/get-by-id/{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await _advertisementService.GetPostByIdAsync(id);
            var postResponse = _mapper.Map<AdvertisementPostResponse>(post);
            return Ok(postResponse);
        }

        [HttpPut("partner/post/update/{postId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromForm] UpdateAdvertisementPostRequest request)
        {
            var updated = await _advertisementService.UpdatePostAsync(postId, request);
            var postResponse = _mapper.Map<AdvertisementPostResponse>(updated);
            return Ok(postResponse);
        }
    }
}
