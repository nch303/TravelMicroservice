using MessageService.Application.Hubs;
using MessageService.Application.IServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Services
{
    public class SignalRRealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRRealtimeNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(Guid groupId, object message)
        {
            await _hubContext.Clients.Group(groupId.ToString())
                                     .SendAsync("ReceiveMessage", message);
        }

        public async Task NotifyReactionUpdatedAsync(Guid groupId, object reaction)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("ReactionUpdated", reaction);
        }

        public async Task NotifyMessageEditedAsync(Guid groupId, object message)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("MessageEdited", message);
        }

        public async Task NotifyMessageDeletedAsync(Guid groupId, Guid messageId)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("MessageDeleted", messageId);
        }

        public async Task NotifyMessageReadAsync(Guid groupId, Guid messageId, Guid userId)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("MessageRead", messageId, userId);
        }
    }
}
