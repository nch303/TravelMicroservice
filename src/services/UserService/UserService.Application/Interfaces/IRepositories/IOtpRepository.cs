using UserService.Domain.Entities;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces.IRepositories
{
    public interface IOtpRepository
    {
        Task AddAsync(OtpVerification otp);
        Task<OtpVerification?> GetValidOtpAsync(string email, string otpCode, string purpose);
        Task SaveChangesAsync();
    }
}
