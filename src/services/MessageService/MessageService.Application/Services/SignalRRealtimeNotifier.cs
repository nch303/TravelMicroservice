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

        public async Task EditMessageAsync(Guid groupId, object message)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("EditMessage", message);
        }

        public async Task AddReactionAsync(Guid groupId, object reactionInfo)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("AddReaction", reactionInfo);
        }

        public async Task RemoveReactionAsync(Guid groupId, object reactionInfo)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("RemoveReaction", reactionInfo);
        }

        public async Task ReadMessageAsync(Guid groupId, object readInfo)
        {
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync("ReadMessage", readInfo);
        }
    }
}
