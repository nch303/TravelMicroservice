using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface IReactionService
    {
        Task AddReactionAsync(Guid userId, MessageReaction messageReaction);
        Task<MessageReaction> RemoveReactionAsync(Guid reactionId, Guid userId);
        Task<List<MessageReaction>?> GetReactionsByMessageIdAsync(Guid messageId);
    }
}
