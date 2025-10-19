using AdminService.Application.DTOs.Requests;
using AdminService.Application.IServiceClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AdminService.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IScheduleServiceClient _scheduleServiceClient;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IAdvertisementServiceClient _packageServiceClient;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IUserServiceClient _userServiceClient;

        public AdminController(IScheduleServiceClient scheduleServiceClient, IAuthServiceClient authServiceClient
            , IAdvertisementServiceClient packageServiceClient, IPaymentServiceClient paymentServiceClient
            , IUserServiceClient userServiceClient)
        {
            _scheduleServiceClient = scheduleServiceClient;
            _authServiceClient = authServiceClient;
            _packageServiceClient = packageServiceClient;
            _paymentServiceClient = paymentServiceClient;
            _userServiceClient = userServiceClient;
        }

        [HttpGet("account/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAccountsAsync()
        {
            try
            {
                var accounts =  await _authServiceClient.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("profile/by-id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfileAsync(Guid userId)
        {
            try
            {
                var profile = await _userServiceClient.GetProfileAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("schedule/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSchedulesAsync()
        {
            try
            {
                var schedules = await _scheduleServiceClient.GetAllSchedulesAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("package/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPackagesAsync()
        {
            try
            {
                var packages = await _packageServiceClient.GetAllPackagesAsync();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("package/get-by-id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPackageByIdAsync(Guid packageId)
        {
            try
            {
                var package = await _packageServiceClient.GetPackageByIdAsync(packageId);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("package/create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePackageAsync(CreatePackageRequest request)
        {
            try
            {
                var package = await _packageServiceClient.CreatePackageAsync(request);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("package/update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackageAsync(Guid id, UpdatePackageRequest request)
        {
            try
            {
                var package = await _packageServiceClient.UpdatePackageAsync(id, request);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("package/delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackageAsync(Guid id)
        {
            try
            {
                var package = await _packageServiceClient.DeletePackageAsync(id);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("package/restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestorePackageAsync(Guid id)
        {
            try
            {
                var package = await _packageServiceClient.RestorePackageAsync(id);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("post/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPostAsync()
        {
            try
            {
                var posts = await _packageServiceClient.GetAllPostAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("transaction/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTransactionAsync()
        {
            try
            {
                var transactions = await _paymentServiceClient.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
