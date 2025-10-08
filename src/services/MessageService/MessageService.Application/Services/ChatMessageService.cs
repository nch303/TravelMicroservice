using MessageService.Application.DTOs.Responses;
using MessageService.Application.IServiceClients;
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
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IChatGroupRepository _chatGroupRepository;

        public ChatMessageService(IChatMessageRepository chatMessageRepository, IChatParticipantRepository chatParticipantRepository
            , INotificationRepository notificationRepository, IUserServiceClient userServiceClient
            , IChatGroupRepository chatGroupRepository)
        {
            _chatMessageRepository = chatMessageRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _notificationRepository = notificationRepository;
            _userServiceClient = userServiceClient;
            _chatGroupRepository = chatGroupRepository;
        }

        public async Task SendMessageAsync(ChatMessage chatMessage)
        {
            // Kiểm tra sender là participant
            var participant = await _chatParticipantRepository
                .GetParticipantAsync(chatMessage.GroupId, chatMessage.SenderId);

            if (participant == null || participant.Status != ParticipantStatus.Active)
                throw new InvalidOperationException("The sender is not a participant of the chat group.");

            // Kiểm tra message content
            if (string.IsNullOrWhiteSpace(chatMessage.Content) && chatMessage.MessageType == MessageType.Text)
                throw new InvalidOperationException("Message content cannot be empty for text messages.");

            chatMessage.SenderId = participant.Id;

            // Lưu message
            await _chatMessageRepository.SendMessageAsync(chatMessage);

            // Lấy các participant active (ngoại trừ sender)
            var activeParticipants = await _chatParticipantRepository
                .GetActiveParticipantsByGroupIdAsync(chatMessage.GroupId);

            var recipientIds = activeParticipants
                .Where(p => p.Id != chatMessage.SenderId)
                .Select(p => p.UserId)
                .ToList();

            if (!recipientIds.Any())
                return; // Không còn ai nhận notification

            // Lấy profile batch
            var senderProfile = await _userServiceClient.GetUserProfileAsync(participant.UserId);

            // Lấy Group info
            var group = await _chatGroupRepository.GetGroupByIdAsync(chatMessage.GroupId);
            if (group == null)
                throw new InvalidOperationException("Chat group does not exist.");

            // Tạo notification
            var notifications = recipientIds.Select(userId =>
            {
                return new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ChatMessageId = chatMessage.Id,
                    ChatGroupId = chatMessage.GroupId,
                    Title = $"Bạn có 1 tin nhắn mới từ [{group.Name}]",
                    Content = $"{senderProfile!.Name}: {chatMessage.Content}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
            }).ToList();

            // Bulk insert hoặc parallel insert
            await _notificationRepository.BulkInsertNotificationsAsync(notifications);
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
