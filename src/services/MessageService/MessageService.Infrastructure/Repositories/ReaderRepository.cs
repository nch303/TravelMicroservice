using MessageService.Domain.Entities;
using MessageService.Domain.IRepositories;
using MessageService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace MessageService.Infrastructure.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly AppDbContext _context;

        public ReaderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task MarkAsReadAsync(MessageRead messageRead)
        {
            _context.MessageReads.Add(messageRead);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatMessage>?> GetUnreadMessagesAsync(Guid groupId, Guid readerId)
        {
            return await _context.ChatMessages
                        .Where(m => m.GroupId == groupId &&
                                    !m.Reads.Any(r => r.ReaderId == readerId))
                        .ToListAsync();
        }

        public async Task<List<MessageRead>> GetMessageReadsByMessageIdAsync(Guid messageId)
        {
            return await _context.MessageReads
                        .Include(mr => mr.Reader) // Include the Reader navigation property
                        .Where(mr => mr.MessageId == messageId)
                        .ToListAsync();
        }
    }
}
