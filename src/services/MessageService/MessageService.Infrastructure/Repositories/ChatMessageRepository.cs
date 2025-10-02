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
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _context;
        public ChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task SendMessageAsync(ChatMessage chatMessage)
        {
            await _context.ChatMessages.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ChatMessage?> GetMessageByIdAsync(Guid messageId)
        {
            return await _context.ChatMessages.FindAsync(messageId);
        }

        public async Task<List<ChatMessage>> GetMessagesByGroupIdAsync(Guid groupId, DateTime? beforeCreatedAt, int pageSize)
        {
            IOrderedQueryable<ChatMessage> query = _context.ChatMessages
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.CreatedAt);

            if (beforeCreatedAt.HasValue)
            {
                query = query.Where(m => m.CreatedAt < beforeCreatedAt.Value)
                             .OrderByDescending(m => m.CreatedAt); // Ensure the query remains ordered
            }

            var messages =  await query
                .Take(pageSize)
                .ToListAsync();

            // Đảo lại tăng dần (ASC) trước khi return
            return messages.OrderBy(m => m.CreatedAt).ToList();
        }


    }
}
