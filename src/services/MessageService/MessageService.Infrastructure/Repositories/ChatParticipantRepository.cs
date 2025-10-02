using MessageService.Domain.Entities;
using MessageService.Domain.IRepositories;
using MessageService.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Infrastructure.Repositories
{
    public class ChatParticipantRepository: IChatParticipantRepository
    {
        private readonly AppDbContext _context;

        public ChatParticipantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddParticipantAsync(ChatParticipant chatParticipant)
        {
            await _context.ChatParticipants.AddAsync(chatParticipant);
            await _context.SaveChangesAsync();
        }

        public async Task<ChatParticipant?> GetParticipantAsync(Guid chatGroupId, Guid participantId)
        {
            return await Task.FromResult(_context.ChatParticipants
                .FirstOrDefault(p => p.ChatGroupId == chatGroupId && p.ParticipantId == participantId));
        }
    }
}
