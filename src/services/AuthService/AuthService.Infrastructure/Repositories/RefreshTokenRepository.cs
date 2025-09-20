using AuthService.Domain.Entities;
using AuthService.Domain.IRepositories;
using AuthService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            return refreshToken!;
        }
        public async Task RevokeTokenAsync(RefreshToken refreshToken)
        {
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken.Token);

            existingToken!.IsRevoked = true;
            _context.RefreshTokens.Update(existingToken);
            await _context.SaveChangesAsync();
        }
    }
}
