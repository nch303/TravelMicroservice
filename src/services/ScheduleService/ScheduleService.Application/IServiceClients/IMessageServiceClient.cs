using ScheduleService.Application.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.IServiceClients
{
    public interface IMessageServiceClient
    {
        Task<object> CreateMessageGroupAsync(CreateChatGroupRequest request);
    }
}
