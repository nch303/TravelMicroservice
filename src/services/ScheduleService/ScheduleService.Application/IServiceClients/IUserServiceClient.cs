using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.DTOs.Responses;

namespace ScheduleService.Application.IServiceClients
{
    public interface IUserServiceClient
    {
        Task<List<UserServiceClientResponse>> GetUsersByIdsAsync(List<Guid> userIds);
    }
}
