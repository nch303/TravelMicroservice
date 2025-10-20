using MessageService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServiceClients
{
    public interface IAuthServiceClient
    {
        Task<AccountResponse?> GetCurrentAccountAsync();
    }
}
