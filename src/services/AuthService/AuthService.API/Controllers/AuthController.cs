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
            try
            {
                await _authService.RegisterAsync(registerDto);
                return Ok(new { Message = "Đã gửi OTP về email của bạn" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }


        [HttpPost("verify-register-otp")]
        public async Task<IActionResult> VerifyRegisterOtp([FromBody] OtpRequest dto)
        {
            try
            {
                var success = await _authService.GetValidOtpAsync(dto.Email, dto.OtpCode, "Register");
                if (!success) return BadRequest(new { Message = "OTP không hợp lệ hoặc đã hết hạn" });
                return Ok(new { Message = "Tài khoản đã được kích hoạt" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            try
            {
                await _authService.SendResetOtpAsync(email);
                return Ok(new { message = "Đã gửi mã OTP về email của bạn" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _authService.ResetPasswordAsync(request.Email, request.OtpCode, request.NewPassword);
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
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
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
