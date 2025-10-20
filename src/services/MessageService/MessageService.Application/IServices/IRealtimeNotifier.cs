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
        Task EditMessageAsync(Guid groupId, object message);
        Task AddReactionAsync(Guid groupId, object reactionInfo);
        Task RemoveReactionAsync(Guid groupId, object reactionInfo);
        Task ReadMessageAsync(Guid groupId, object readInfo);
    }
}
