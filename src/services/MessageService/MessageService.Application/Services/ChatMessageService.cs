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
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;

        public ChatMessageService(IChatMessageRepository chatMessageRepository, IChatParticipantRepository chatParticipantRepository)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatParticipantRepository = chatParticipantRepository;
        }

        public async Task SendMessageAsync(ChatMessage chatMessage)
        {
            // Check if sender is part of the chat group
            var participant = await _chatParticipantRepository.GetParticipantAsync(chatMessage.GroupId, chatMessage.SenderId);
            if (participant == null || participant.Status != ParticipantStatus.Active)
            {
                throw new InvalidOperationException("The sender is not a participant of the chat group.");
            }

            // Check non-content message
            if (string.IsNullOrWhiteSpace(chatMessage.Content) && chatMessage.MessageType == MessageType.Text)
            {
                throw new InvalidOperationException("Message content cannot be empty for text messages.");
            }

            chatMessage.SenderId = participant!.Id;
            await _chatMessageRepository.SendMessageAsync(chatMessage);
        }

        public async Task EditMessageAsync(ChatMessage chatMessage)
        {
            var existingMessage = await _chatMessageRepository.GetMessageByIdAsync(chatMessage.Id);
            if (existingMessage == null)
            {
                throw new InvalidOperationException("The message does not exist.");
            }

            // Only the sender can edit the message
            if (existingMessage.SenderId != chatMessage.SenderId)
            {
                throw new InvalidOperationException("Only the sender of this message can edit it.");
            }

            // Check non-content message
            if (string.IsNullOrWhiteSpace(chatMessage.Content) && chatMessage.MessageType == MessageType.Text)
            {
                throw new InvalidOperationException("Message content cannot be empty for text messages.");
            }

            // Check parent message if it's a reply
            if (chatMessage.ParentMessageId.HasValue)
            {
                var parentMessage = await _chatMessageRepository.GetMessageByIdAsync(chatMessage.ParentMessageId.Value);
                if (parentMessage == null || parentMessage.GroupId != existingMessage.GroupId)
                {
                    throw new InvalidOperationException("The parent message does not exist in the same chat group.");
                }
            }

            existingMessage.Content = chatMessage.Content;
            existingMessage.EditAt = DateTime.UtcNow;
            await _chatMessageRepository.SaveChangeAsync();
        }

        public async Task<List<ChatMessage>> GetMessagesByGroupIdAsync(Guid groupId, DateTime? beforeCreatedAt, int pageSize)
        {
            return await _chatMessageRepository.GetMessagesByGroupIdAsync(groupId, beforeCreatedAt, pageSize);
        }

        public async Task<ChatMessage?> GetMessageByIdAsync(Guid messageId)
        {
            return await _chatMessageRepository.GetMessageByIdAsync(messageId);
        }
    }
}
