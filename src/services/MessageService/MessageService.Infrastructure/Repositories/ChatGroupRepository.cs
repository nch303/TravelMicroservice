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
    public class ChatGroupRepository : IChatGroupRepository
    {
        private readonly AppDbContext _context;

        public ChatGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateGroupAsync(ChatGroup chatGroup)
        {        
            _context.ChatGroups.Add(chatGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatGroup>> GetUserGroupsAsync(Guid userId)
        {
            return await Task.FromResult(_context.ChatGroups
                .Where(g => g.Participants.Any(p => p.ParticipantId == userId))
                .ToList());
        }

        public async Task<ChatGroup?> GetGroupByIdAsync(Guid groupId)
        {
            return await _context.ChatGroups.FindAsync(groupId);
        }

        public async Task<ChatGroup?> GetGroupByScheduleIdAsync(Guid? scheduleId)
        {
            return await Task.FromResult(_context.ChatGroups
                .FirstOrDefault(g => g.ScheduleId == scheduleId));
        }
    }
}
