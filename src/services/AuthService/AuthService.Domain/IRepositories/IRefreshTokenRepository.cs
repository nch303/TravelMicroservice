using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task CreateTokenAsync(RefreshToken refreshToken);
        Task RevokeTokenAsync(RefreshToken refreshToken);
    }
}
