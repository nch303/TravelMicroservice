﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Requests
{
    public class ResetPasswordRequest
    {
        public string resetToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
