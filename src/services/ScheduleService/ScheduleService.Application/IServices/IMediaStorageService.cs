using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ScheduleService.Application.IServices
{
    public interface IMediaStorageService
    {
        Task<string> SaveAsync(IFormFile file); // trả URL công khai
    }
}
