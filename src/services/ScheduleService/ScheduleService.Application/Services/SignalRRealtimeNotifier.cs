using Microsoft.AspNetCore.SignalR;
using ScheduleService.Application.Hubs;
using ScheduleService.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Services
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendGroupNotificationAsync(Guid scheduleId, object notification)
        {
            await _hubContext.Clients.Group(scheduleId.ToString())
                                     .SendAsync("ReceiveNotification", notification);
        }

        public async Task SendUserNotificationAsync(Guid userId, object notification)   
        {
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("ReceiveNotification", notification);
        }
    }
}
