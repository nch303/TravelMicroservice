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
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;

        public ReaderService(IReaderRepository readerRepository, IChatMessageRepository chatMessageRepository
            , IChatParticipantRepository chatParticipantRepository)
        {
            _readerRepository = readerRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatParticipantRepository = chatParticipantRepository;
        }

        public async Task MarkAsReadAsync(Guid groupId, Guid userId)
        {
            // Check if the user is a participant of the chat group
            var participant = await _chatParticipantRepository.GetParticipantAsync(groupId, userId);
            if (participant == null)
            {
                throw new Exception("You are not a participant of this chat group.");
            }

            // Get all unread messages for the user in the group
            var unreadMessages = await _readerRepository.GetUnreadMessagesAsync(groupId, participant.Id);
            if (unreadMessages == null || !unreadMessages.Any())
            {
                return;
            }

            

            // Mark each unread message as read
            foreach (var message in unreadMessages)
            {
                var messageRead = new MessageRead
                {
                    Id = Guid.NewGuid(),
                    MessageId = message.Id,
                    ReaderId = participant.Id,
                    ReadAt = DateTime.UtcNow
                };
                await _readerRepository.MarkAsReadAsync(messageRead);
            }

        }

        public async Task<List<ChatMessage>?> GetUnreadMessagesAsync(Guid groupId, Guid userId)
        {
            // Check if the user is a participant of the chat group
            var participant = await _chatParticipantRepository.GetParticipantAsync(groupId, userId);
            if (participant == null)
            {
                throw new Exception("You are not a participant of this chat group.");
            }
            // Get unread messages for the user in the group
            return await _readerRepository.GetUnreadMessagesAsync(groupId, participant.Id);
        }

        public async Task<List<MessageRead>> GetMessageReadsByMessageIdAsync(Guid messageId)
        {
            return await _readerRepository.GetMessageReadsByMessageIdAsync(messageId);
        }
    }
}
