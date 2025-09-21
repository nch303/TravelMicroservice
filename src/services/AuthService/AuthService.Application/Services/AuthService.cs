using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Application.IServiceClients;
using AuthService.Application.IServices;
using AuthService.Domain.Entities;
using AuthService.Domain.IRepositories;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class AuthServices: IAuthService
    {
        private readonly IAuthRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthServices(IAuthRepository userRepository, IConfiguration configuration, 
            IOtpRepository optRepository, IEmailService emailService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _otpRepository = optRepository;
            _emailService = emailService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task RegisterAsync(Account newAccount)
        {
            var existingUser = await _userRepository.GetByEmailAsync(newAccount.Email);
            if (existingUser != null)
                throw new Exception("Email đã tồn tại");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(newAccount.PasswordHash);

            var account = new Account 
            { 
                Id = newAccount.Id,
                Email = newAccount.Email, 
                PasswordHash = passwordHash,
                IsActive = false,
                RoleId = 2 // Mặc định role user
            };
            await _userRepository.AddAsync(account);
            await _userRepository.SaveChangesAsync();

            // Tạo OTP
            var otpCode = GenerateOtp();
            var otpEntity = new OtpVerification
            {
                Email = account.Email,
                OtpCode = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Purpose = "Register"
            };
            await _otpRepository.AddAsync(otpEntity);
            await _otpRepository.SaveChangesAsync();

            // Gửi email OTP
            //await SendOtpEmail(account.Email, otpCode);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginDto)
        {
            var account = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (account == null || BCrypt.Net.BCrypt.Verify(loginDto.Password, account.PasswordHash) == false)
                throw new InvalidOperationException("Email hoặc password sai");

            if (account!.IsActive == false)
                throw new Exception("Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để xác nhận tài khoản.");

            var token = GenerateJwtToken(account);

            // Tạo refresh token
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                InitialLoginAt = DateTime.UtcNow,
                AccountId = account.Id,
                IsRevoked = false
            };
            await _refreshTokenRepository.CreateTokenAsync(refreshToken);

            int expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!);

            return new AuthResponse
            {
                Token = token,
                UserName = account.Email,
                ExpiresAt = DateTime.Now.AddMinutes(expiryMinutes),
                RefreshToken = refreshToken.Token
            };
        }

        private string GenerateJwtToken(Account account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email)
            };

            int expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryInMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateOtp(int length = 6)
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, length).Select(_ => random.Next(0, 10)));
        }

        private async Task SendOtpEmail(string email, string otp)
        {
            var subject = "Mã OTP xác nhận tài khoản";
            var body = $"Mã OTP của bạn là <b>{otp}</b>. Hạn dùng: 5 phút.";
            await _emailService.SendEmailAsync(email, subject, body);
        }

        public async Task<bool> GetValidOtpAsync(string email, string otpCode, string purpose)
        {
            var otp = await _otpRepository.GetValidOtpAsync(email, otpCode, purpose);
            if (otp == null)
                return false;

            otp.IsUsed = true;
            await _otpRepository.SaveChangesAsync();

            // Kích hoạt user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null) user.IsActive = true;
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task SendResetOtpAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Exception("Email không tồn tại");

            var otp = new OtpVerification
            {
                Email = email,
                OtpCode = new Random().Next(100000, 999999).ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                Purpose = "ResetPassword"
            };

            await _otpRepository.AddAsync(otp);
            await _otpRepository.SaveChangesAsync();

            string subject = "OTP Reset Password";
            string body = $"Mã OTP của bạn là: <b>{otp.OtpCode}</b>. Hết hạn trong 10 phút.";
            await _emailService.SendEmailAsync(email, subject, body);
        }

        public async Task ResetPasswordAsync(string email, string otpCode, string newPassword)
        {
            var otp = await _otpRepository.GetValidOtpAsync(email, otpCode, "ResetPassword");
            if (otp == null) throw new Exception("OTP không hợp lệ hoặc hết hạn");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Exception("Người dùng không tồn tại");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            otp.IsUsed = true;

            await _userRepository.SaveChangesAsync();
            await _otpRepository.SaveChangesAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("Người dùng không tồn tại");
            return user;
        }

        public async Task ChangePasswordAsync(Account user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null) throw new Exception("Người dùng không tồn tại");
            existingUser.PasswordHash = user.PasswordHash;
            // Cập nhật các trường khác nếu cần
            await _userRepository.ChangePasswordAsync(user);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken, Account account)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if(storedToken == null || storedToken.AccountId != account.Id)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (storedToken == null || storedToken.IsRevoked)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            // ✅ Kiểm tra absolute expiration (ví dụ 90 ngày)
            int absoluteDays = int.Parse(_configuration["JwtSettings:AbsoluteExpiryDays"] ?? "90");
            if (DateTime.UtcNow > storedToken.InitialLoginAt.AddDays(absoluteDays))
                throw new UnauthorizedAccessException("Session expired, please login again.");

            // Nếu hợp lệ → cấp AccessToken + RefreshToken mới
            var newAccessToken = GenerateJwtToken(account); // access token mới

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                InitialLoginAt = storedToken.InitialLoginAt, // Giữ nguyên mốc login ban đầu
                AccountId = account.Id,
                IsRevoked = false
            };

            storedToken.IsRevoked = true; // hủy token cũ
            await _refreshTokenRepository.RevokeTokenAsync(storedToken); // hủy token cũ 
            await _refreshTokenRepository.CreateTokenAsync(newRefreshToken); // lưu token mới

            return (newAccessToken, newRefreshToken.Token);
        }

    }
}
