using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
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

        public async Task<ChatParticipant?> GetParticipantAsync(Guid chatGroupId, Guid userId)
        {
            return await Task.FromResult(_context.ChatParticipants
                .FirstOrDefault(p => p.ChatGroupId == chatGroupId && p.UserId == userId));
        }

        public async Task<ChatParticipant?> GetParticipantByIdAsync(Guid participantId)
        {
            return await _context.ChatParticipants.FindAsync(participantId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatParticipant>> GetActiveParticipantsByGroupIdAsync(Guid chatGroupId)
        {
            return await Task.FromResult(_context.ChatParticipants
                                                    .Where(p => p.ChatGroupId == chatGroupId && p.Status == ParticipantStatus.Active)
                                                    .ToList());
        }
    }
}
