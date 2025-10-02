using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Services
{
    public class ChatParticipantService: IChatParticipantService
    {
        private readonly IChatParticipantRepository _chatParticipantRepository;

        public ChatParticipantService(IChatParticipantRepository chatParticipantRepository)
        {
            _chatParticipantRepository = chatParticipantRepository;
        }

        public async Task AddParticipantAsync(ChatParticipant chatParticipant)
        {
            var existingParticipant = await _chatParticipantRepository.GetParticipantAsync(chatParticipant.ChatGroupId, chatParticipant.ParticipantId);
            if (existingParticipant != null)
            {
                throw new InvalidOperationException("The participant is already in the group.");
            }

            await _chatParticipantRepository.AddParticipantAsync(chatParticipant);
        }

        public async Task<ChatParticipant?> GetParticipantAsync(Guid chatGroupId, Guid participantId)
        {
            return await _chatParticipantRepository.GetParticipantAsync(chatGroupId, participantId);
        }
    }
}
