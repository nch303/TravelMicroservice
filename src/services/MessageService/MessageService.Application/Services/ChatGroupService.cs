using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
using MessageService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Services
{
    public class ChatGroupService: IChatGroupService
    {
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;

        public ChatGroupService(IChatGroupRepository chatGroupRepository, IChatParticipantRepository chatParticipantRepository)
        {
            _chatGroupRepository = chatGroupRepository;
            _chatParticipantRepository = chatParticipantRepository;
        }

        public async Task CreateGroupAsync(ChatGroup chatGroup)
        {        
            if (chatGroup.ScheduleId != null)
            {
                var existingGroup = await _chatGroupRepository.GetGroupByScheduleIdAsync(chatGroup.ScheduleId);
                if (existingGroup != null)
                {
                    throw new InvalidOperationException("This schedule already had a chat group.");
                }
            }
            await _chatGroupRepository.CreateGroupAsync(chatGroup);

            // Add onwer as a participant
            await _chatParticipantRepository.AddParticipantAsync(new ChatParticipant
            {
                Id = Guid.NewGuid(),
                ChatGroupId = chatGroup.Id,
                ParticipantId = chatGroup.UserId,
                JoinedAt = DateTime.UtcNow,
                Role = ParticipantRole.Owner,
                Status = ParticipantStatus.Active,
                LastSeenAt = DateTime.UtcNow
            });
        }

        public async Task<List<ChatGroup>> GetUserGroupsAsync(Guid userId)
        {
            return await _chatGroupRepository.GetUserGroupsAsync(userId);
        }

        public async Task<ChatGroup?> GetGroupByIdAsync(Guid groupId)
        {
            return await _chatGroupRepository.GetGroupByIdAsync(groupId);
        }

        public async Task<ChatGroup?> GetGroupByScheduleIdAsync(Guid? scheduleId)
        {
            return await _chatGroupRepository.GetGroupByScheduleIdAsync(scheduleId);
        }
    }
}
