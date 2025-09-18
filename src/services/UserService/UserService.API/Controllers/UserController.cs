using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;


[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerDto)
    {
        await _userService.RegisterAsync(registerDto);
        return Ok(new { Message = "Đã gửi OTP về email của bạn" });
    }

    [HttpPost("verify-register-otp")]
    public async Task<IActionResult> VerifyRegisterOtp([FromBody] OtpRequest dto)
    {
        var success = await _userService.GetValidOtpAsync(dto.Email, dto.OtpCode, "Register");
        if (!success) return BadRequest(new { Message = "OTP không hợp lệ hoặc đã hết hạn" });
        return Ok(new { Message = "Tài khoản đã được kích hoạt" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
    {
        var result = await _userService.LoginAsync(loginDto);
        return Ok(result);
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
    {
        await _userService.SendResetOtpAsync(email);
        return Ok(new { message = "Đã gửi mã OTP về email của bạn" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _userService.ResetPasswordAsync(request.Email, request.OtpCode, request.NewPassword);
        return Ok(new { message = "Đổi mật khẩu thành công" });
    }
}