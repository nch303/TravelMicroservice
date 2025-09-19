using AuthService.Application.DTOs.Requests;
using AuthService.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerDto)
        {
            await _authService.RegisterAsync(registerDto);
            return Ok(new { Message = "Đã gửi OTP về email của bạn" });
        }


        [HttpPost("verify-register-otp")]
        public async Task<IActionResult> VerifyRegisterOtp([FromBody] OtpRequest dto)
        {
            var success = await _authService.GetValidOtpAsync(dto.Email, dto.OtpCode, "Register");
            if (!success) return BadRequest(new { Message = "OTP không hợp lệ hoặc đã hết hạn" });
            return Ok(new { Message = "Tài khoản đã được kích hoạt" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            await _authService.SendResetOtpAsync(email);
            return Ok(new { message = "Đã gửi mã OTP về email của bạn" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request.Email, request.OtpCode, request.NewPassword);
            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            // 1. Lấy userId từ JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user id in token");

            // 2. Tìm user trong DB
            var user = await _authService.GetByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            // Verify khi login/đổi mật khẩu
            bool valid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
            if (!valid)
                return BadRequest("Old password is incorrect");

            // 4. Hash mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // 5. Cập nhật DB
            await _authService.UpdateUser(user);

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
