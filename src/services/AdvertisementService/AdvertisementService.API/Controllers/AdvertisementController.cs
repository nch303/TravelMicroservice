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
            try
            {
                var packages = await _packageService.GetAllAsync();
                var packagesResponse = _mapper.Map<List<PackageResponse>>(packages);
                return Ok(packagesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("package/get-by-id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var pkg = await _packageService.GetByIdAsync(id);
                var packageResponse = _mapper.Map<PackageResponse>(pkg);
                return Ok(packageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("package/create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePackageRequest request)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("package/update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePackageRequest request)
        {
            try
            {
                var package = _mapper.Map<AdvertisementPackage>(request);
                var updated = await _packageService.UpdateAsync(id, package);
                var packageResponse = _mapper.Map<PackageResponse>(updated);
                return Ok(packageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }

        [HttpDelete("package/delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _packageService.DeleteAsync(id);
                var package = await _packageService.GetByIdAsync(id);
                var response = _mapper.Map<PackageResponse>(package);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("package/restore/{id}")]
        [Authorize]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var package = await _packageService.RestoreAsync(id);
                var response = _mapper.Map<PackageResponse>(package);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy danh sách gói đã mua của Partner
        [HttpGet("partner/purchase/get-by-partnerId/{partnerId}")]
        public async Task<IActionResult> GetPurchaseByPartner(Guid partnerId)
        {
            try
            {
                var purchases = await _purchaseService.GetPurchasesByPartnerAsync(partnerId);
                var purchasesResponse = _mapper.Map<List<PurchaseResponse>>(purchases);
                return Ok(purchasesResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy chi tiết một gói đã mua
        [HttpGet("partner/purchase/get-by-id/{id}")]
        public async Task<IActionResult> GetByPurchaseById(Guid id)
        {
            try
            {
                var purchase = await _purchaseService.GetByIdAsync(id);
                if (purchase == null) return NotFound();

                var purchaseResponse = _mapper.Map<PurchaseResponse>(purchase);
                return Ok(purchaseResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Mua gói quảng cáo (chưa tích hợp thanh toán)
        [HttpPost("partner/purchase/create")]
        public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request)
        {
            try
            {
                var purchase = await _purchaseService.CreatePurchaseAsync(request.UserId, request.PackageId, request.TransactionId);
                var purchaseResponse = _mapper.Map<PurchaseResponse>(purchase);
                return Ok(purchaseResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("partner/post/create")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] CreateAdvertisementPostRequest request)
        {
            try
            {
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                // Gọi service xử lý toàn bộ logic (upload + lưu DB)
                var post = await _advertisementService.CreatePostAsync(currentAccount.Id, request);
                var postResponse = _mapper.Map<AdvertisementPostResponse>(post);
                return Ok(postResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("partner/post/get-by-partnerId/{partnerId}")]
        public async Task<IActionResult> GetPostsByPartner(Guid partnerId)
        {
            try
            {
                var posts = await _advertisementService.GetPostsByPartnerAsync(partnerId);
                var postsResponse = _mapper.Map<List<AdvertisementPostResponse>>(posts);
                return Ok(postsResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("post/all")]
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
            try
            {
                var post = await _advertisementService.GetPostByIdAsync(id);
                var postResponse = _mapper.Map<AdvertisementPostResponse>(post);
                return Ok(postResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("partner/post/update/{postId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromForm] UpdateAdvertisementPostRequest request)
        {
            try
            {
                var updated = await _advertisementService.UpdatePostAsync(postId, request);
                var postResponse = _mapper.Map<AdvertisementPostResponse>(updated);
                return Ok(postResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
