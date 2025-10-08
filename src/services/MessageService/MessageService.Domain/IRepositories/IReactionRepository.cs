using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.IRepositories
{
    public interface IReactionRepository
    {
        Task AddReactionAsync(MessageReaction messageReaction);
        Task<MessageReaction?> GetReactionsByMessageIdAndUserIdAsync(Guid messageId, Guid userId);
        Task SaveChanges();
        Task<MessageReaction?> GetReactionsByIdAsync(Guid reactionId);
        Task<List<MessageReaction>?> GetReactionsByMessageIdAsync(Guid messageId);

    }
}
