using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;
using UserService.Application.DTOs.Responses;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest registerDto);
        Task<AuthResponse> LoginAsync(LoginRequest loginDto);
    }
}