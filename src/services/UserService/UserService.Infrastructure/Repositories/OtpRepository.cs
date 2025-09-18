using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.IRepositories;
using UserService.Domain.Entities;

public class OtpRepository : IOtpRepository
{
    private readonly AppDbContext _context;
    public OtpRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(OtpVerification otp)
    {
        await _context.OtpVerifications.AddAsync(otp);
    }

    public async Task<OtpVerification?> GetValidOtpAsync(string email, string otpCode, string purpose)
    {
        return await _context.OtpVerifications
            .FirstOrDefaultAsync(
                o => o.Email == email && 
                o.OtpCode == otpCode && 
                !o.IsUsed && 
                o.ExpiresAt > DateTime.UtcNow &&
                o.Purpose == purpose);
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
