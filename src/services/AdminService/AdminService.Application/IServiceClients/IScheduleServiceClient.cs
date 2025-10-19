using AdminService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminService.Application.IServiceClients
{
    public interface IScheduleServiceClient
    {
        Task<List<ScheduleResponse>> GetAllSchedulesAsync();
    }
}
