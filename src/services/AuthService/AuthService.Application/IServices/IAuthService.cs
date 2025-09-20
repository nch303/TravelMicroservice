using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.IServices
{
    public interface IAuthService
    {
        Task RegisterAsync(Account account);
        Task<bool> GetValidOtpAsync(string email, string otpCode, string purpose);
        Task<AuthResponse> LoginAsync(LoginRequest loginDto);
        Task SendResetOtpAsync(string email);
        Task ResetPasswordAsync(string email, string otpCode, string newPassword);
        Task<Account?> GetByIdAsync(Guid id);
        Task ChangePasswordAsync(Account user);
        Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, Account account);
    }
}
