using MessageService.Domain.Entities;
using MessageService.Domain.IRepositories;
using MessageService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Infrastructure.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly AppDbContext _context;

        public ReactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddReactionAsync(MessageReaction messageReaction)
        {
            await _context.MessageReactions.AddAsync(messageReaction);
            await _context.SaveChangesAsync();
        }

        public async Task<MessageReaction?> GetReactionsByMessageIdAndUserIdAsync(Guid messageId, Guid userId)
        {
            return await _context.MessageReactions
                .FirstOrDefaultAsync(r => r.MessageId == messageId 
                                       && r.ParticipantId == userId
                                       && r.IsDeleted == false);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<MessageReaction?> GetReactionsByIdAsync(Guid reactionId)
        {
            return await _context.MessageReactions
                .Include(r => r.Participant)
                .FirstOrDefaultAsync(r => r.Id == reactionId
                                       && r.IsDeleted == false);
        }

        public async Task<List<MessageReaction>?> GetReactionsByMessageIdAsync(Guid messageId)
        {
            return await _context.MessageReactions
                .Where(r => r.MessageId == messageId
                         && r.IsDeleted == false)
                .ToListAsync();
        }
    }
}
