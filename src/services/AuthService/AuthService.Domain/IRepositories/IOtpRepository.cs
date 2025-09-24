﻿using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.IRepositories
{
    public interface IOtpRepository
    {
        Task AddAsync(OtpVerification otp);
        Task<OtpVerification?> GetValidOtpAsync(string email, string otpCode, string purpose);
        Task<List<OtpVerification>> GetAllByAccountAsync(string email, string purpose);
        Task SaveChangesAsync();

    }
}
