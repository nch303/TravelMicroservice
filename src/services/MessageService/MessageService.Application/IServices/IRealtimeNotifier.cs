using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface IRealtimeNotifier
    {
        Task SendMessageAsync(Guid groupId, object message);
        Task NotifyReactionUpdatedAsync(Guid groupId, object reaction);
        Task NotifyMessageEditedAsync(Guid groupId, object message);
        Task NotifyMessageDeletedAsync(Guid groupId, Guid messageId);
        Task NotifyMessageReadAsync(Guid groupId, Guid messageId, Guid userId);
    }
}
