using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;
using UserService.Application.DTOs.Responses;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterRequest registerDto);
        Task<bool> GetValidOtpAsync(string email, string otpCode, string purpose);
        Task<AuthResponse> LoginAsync(LoginRequest loginDto);
        Task SendResetOtpAsync(string email);
        Task ResetPasswordAsync(string email, string otpCode, string newPassword);
    }
}