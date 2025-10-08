using MessageService.Application.DTOs.Responses;
using MessageService.Application.IServiceClients;
using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
using MessageService.Domain.IRepositories;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Services
{
    public class ChatGroupService : IChatGroupService
    {
        private readonly IChatGroupRepository _chatGroupRepository;
        private readonly IChatParticipantRepository _chatParticipantRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly INotificationRepository _notificationRepository;

        public ChatGroupService(IChatGroupRepository chatGroupRepository, IChatParticipantRepository chatParticipantRepository
            , IChatMessageRepository chatMessageRepository, IUserServiceClient userServiceClient
            , INotificationRepository notificationRepository)
        {
            _chatGroupRepository = chatGroupRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _chatMessageRepository = chatMessageRepository;
            _userServiceClient = userServiceClient;
            _notificationRepository = notificationRepository;
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
                UserId = chatGroup.UserId,
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

        public async Task<ShareGroupResponse> ShareGroupCode(Guid groupId, Guid accountId)
        {
            var group = await _chatGroupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                throw new KeyNotFoundException("Group not found.");
            }

            // Check if the user is a participant of the chat group
            var participant = await _chatParticipantRepository.GetParticipantAsync(groupId, accountId);
            if (participant == null)
            {
                throw new UnauthorizedAccessException("Only the group participants can share the group code.");
            }


            string joinUrl = null;
            string qrBase64 = null;

            if (group.SharedExpired != null && group.SharedExpired > DateTime.UtcNow)
            {
                // Chưa hết hạn, trả về mã cũ
                joinUrl = $"{group.SharedCode}";
                qrBase64 = GenerateQrCodeBase64(joinUrl);
                return new ShareGroupResponse
                {
                    SharedCode = group.SharedCode!,
                    SharedExpired = group.SharedExpired ?? DateTime.UtcNow,
                    QrCodeImage = qrBase64
                };
            }
            else
            {
                // Tạo mã mới (24h)
                group.GenerateRandomCode(24);
                await _chatGroupRepository.SaveChangesAsync();

                // Tạo QR code (chuyển sang base64)
                joinUrl = $"{group.SharedCode}";
                qrBase64 = GenerateQrCodeBase64(joinUrl);

                return new ShareGroupResponse
                {
                    SharedCode = group.SharedCode!,
                    SharedExpired = group.SharedExpired ?? DateTime.UtcNow,
                    QrCodeImage = qrBase64
                };
            }

        }

        private string GenerateQrCodeBase64(string text)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new PngByteQRCode(qrData))
                    {
                        var qrBytes = qrCode.GetGraphic(20);
                        return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
                    }
                }
            }
        }

        public async Task<ChatMessage> JoinGroup(string groupCode, Guid userId)
        {
            var group = await _chatGroupRepository.GetGroupBySharedCodeAsync(groupCode);
            if (group == null)
            {
                throw new KeyNotFoundException("Invalid or expired group code.");
            }

            var userProfile = await _userServiceClient.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                throw new Exception("Can not find profile.");
            }

            // Create message for notice new joiner
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                MessageType = MessageType.Notification,
                Content = $"{userProfile.Name} vừa tham gia nhóm.",
                CreatedAt = DateTime.UtcNow,
                EditAt = null,
                Status = MessageStatus.Sent,
                GroupId = group.Id,
                SenderId = group.Participants.FirstOrDefault(p => p.UserId == userId)!.Id,
                ParentMessageId = null
            };

            // Create notification for notice new joiner
            var notificaton = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ChatGroupId = group.Id,
                ChatMessageId = message.Id,
                Title = "Bạn có 1 tin nhắn mới",
                Content = message.Content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var existingParticipant = await _chatParticipantRepository.GetParticipantAsync(group.Id, userId);
            if (existingParticipant != null)
            {
                if (existingParticipant.Status == ParticipantStatus.Active)
                {
                    throw new InvalidOperationException("You are already a participant of this group.");
                }
                else if (existingParticipant.Status == ParticipantStatus.Banned)
                {
                    throw new InvalidOperationException("You have been banned from this group.");
                }
                else if (existingParticipant.Status == ParticipantStatus.Left)
                {
                    // Re-join the group
                    existingParticipant.Status = ParticipantStatus.Active;
                    existingParticipant.JoinedAt = DateTime.UtcNow;
                    existingParticipant.LastSeenAt = DateTime.UtcNow;
                    await _chatParticipantRepository.SaveChangesAsync();

                    // Create an Noti Message in ChatMessage
                    await _chatMessageRepository.SendMessageAsync(message);

                    // Create Notification
                    await _notificationRepository.CreateNotificationAsync(notificaton);
                }
            }
            else
            {
                // Add new participant
                var newParticipant = new ChatParticipant
                {
                    Id = Guid.NewGuid(),
                    ChatGroupId = group.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                    Role = ParticipantRole.Member,
                    Status = ParticipantStatus.Active,
                    LastSeenAt = DateTime.UtcNow
                };
                await _chatParticipantRepository.AddParticipantAsync(newParticipant);

                // Create an Noti Message in ChatMessage
                await _chatMessageRepository.SendMessageAsync(message);

                // Create Notification
                await _notificationRepository.CreateNotificationAsync(notificaton);
            }
            return message;
        }

        public async Task<ChatMessage> LeaveGroup(Guid groupId, Guid userId)
        {
            var participant = await _chatParticipantRepository.GetParticipantAsync(groupId, userId);
            if (participant == null || participant.Status != ParticipantStatus.Active)
            {
                throw new InvalidOperationException("You are not an active participant of this group.");
            }
            if (participant.Role == ParticipantRole.Owner)
            {
                throw new InvalidOperationException("The owner cannot leave the group. Consider deleting the group instead.");
            }

            // Execute Realtime
            var userProfile = await _userServiceClient.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                throw new Exception("Can not find profile.");
            }

            // Create message for notice new joiner
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                MessageType = MessageType.Notification,
                Content = $"{userProfile.Name} vừa rời nhóm.",
                CreatedAt = DateTime.UtcNow,
                EditAt = null,
                Status = MessageStatus.Sent,
                GroupId = groupId,
                SenderId = participant.Id,
                ParentMessageId = null
            };
            await _chatMessageRepository.SendMessageAsync(message);

            // Create notification for notice new joiner
            var notificaton = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ChatGroupId = groupId,
                ChatMessageId = message.Id,
                Title = "Bạn có 1 tin nhắn mới",
                Content = message.Content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            await _notificationRepository.CreateNotificationAsync(notificaton);

            // Leave group
            participant.Status = ParticipantStatus.Left;
            await _chatParticipantRepository.SaveChangesAsync();

            return message;
        }
    }
}
