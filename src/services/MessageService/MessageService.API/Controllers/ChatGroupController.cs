using AutoMapper;
using Azure.Core;
using Azure.Identity;
using MessageService.Application.DTOs.Requests;
using MessageService.Application.DTOs.Responses;
using MessageService.Application.IServiceClients;
using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MessageService.API.Controllers
{
    [ApiController]
    [Route("api/message")]
    public class ChatGroupController : ControllerBase
    {
        private readonly IChatGroupService _chatGroupService;
        private readonly IMapper _mapper;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IChatMessageService _chatMessageService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IChatParticipantService _chatParticipantService;
        private readonly IReactionService _reactionService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IReaderService _readerService;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly INotificationService _notificationService;

        public ChatGroupController(IChatGroupService chatGroupService, IMapper mapper, IAuthServiceClient authServiceClient
            , IChatMessageService chatMessageService, ICloudinaryService cloudinaryService
            , IChatParticipantService chatParticipantService, IReactionService reactionService, IUserServiceClient userServiceClient
            , IReaderService readerService, IRealtimeNotifier realtimeNotifier, INotificationService notificationService)
        {
            _chatGroupService = chatGroupService;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _chatMessageService = chatMessageService;
            _cloudinaryService = cloudinaryService;
            _chatParticipantService = chatParticipantService;
            _reactionService = reactionService;
            _userServiceClient = userServiceClient;
            _readerService = readerService;
            _realtimeNotifier = realtimeNotifier;
            _notificationService = notificationService;
        }

        private DateTime ConvertToUtc7(DateTime localDateTime)
        {
            // Convert sang giờ VN (UTC+7)
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(localDateTime, vnTimeZone);
            return localTime;
        }

        [HttpPost("group/add")]
        [Authorize]
        public async Task<IActionResult> CreateGroup([FromBody] CreateChatGroupRequest request)
        {
            try
            {
                // Get OwnerId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                {
                    return new UnauthorizedResult();
                }

                var chatGroup = _mapper.Map<ChatGroup>(request);
                chatGroup.Id = Guid.NewGuid();
                chatGroup.UserId = currentAccount.Id; // Set OwnerId
                chatGroup.CreatedAt = DateTime.UtcNow;
                await _chatGroupService.CreateGroupAsync(chatGroup);
                return Ok(new { Message = "Group created successfully", GroupId = chatGroup.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("groups/me")]
        [Authorize]
        public async Task<IActionResult> GetUserGroups()
        {
            // Get UserId By Current Account
            var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
            if (currentAccount == null)
            {
                return new UnauthorizedResult();
            }
            var groups = await _chatGroupService.GetUserGroupsAsync(currentAccount.Id);
            var groupResponses = _mapper.Map<List<ChatGroupResponse>>(groups);
            return Ok(groupResponses);
        }

        [HttpGet("group/{groupId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetGroupById([FromRoute] Guid groupId)
        {
            var group = await _chatGroupService.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return NotFound(new { Message = "Group not found" });
            }
            var groupResponse = _mapper.Map<ChatGroupResponse>(group);
            return Ok(groupResponse);
        }

        [HttpPost("group/share-group/{groupId:guid}")]
        [Authorize]
        public async Task<IActionResult> ShareGroupCode(Guid groupId)
        {
            var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
            if (currentAccount == null)
            {
                return new UnauthorizedResult();
            }

            var sharedCodeObject = await _chatGroupService.ShareGroupCode(groupId, currentAccount.Id);
            sharedCodeObject.SharedExpired = ConvertToUtc7(sharedCodeObject.SharedExpired);
            return Ok(sharedCodeObject);
        }

        [HttpPost("group/join/{sharedCode}")]
        [Authorize]
        public async Task<IActionResult> JoinGroup([FromRoute] string sharedCode)
        {
            try
            {
                // Get UserId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                {
                    return new UnauthorizedResult();
                }
                var chatMessage = await _chatGroupService.JoinGroup(sharedCode, currentAccount.Id);

                var profile = await _userServiceClient.GetUserProfileAsync(currentAccount.Id);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.SendMessageAsync(chatMessage.GroupId, new
                {
                    Type = "JoinGroup",
                    chatMessage.Content,
                    MessageType = chatMessage.MessageType.ToString(),
                    chatMessage.CreatedAt,
                    SenderName = profile!.Name
                });

                return Ok(new { Message = "Joined group successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("group/leave/{groupId:guid}")]
        [Authorize]
        public async Task<IActionResult> LeaveGroup([FromRoute] Guid groupId)
        {
            try
            {
                // Get UserId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                {
                    return new UnauthorizedResult();
                }
                var chatMessage = await _chatGroupService.LeaveGroup(groupId, currentAccount.Id);

                var profile = await _userServiceClient.GetUserProfileAsync(currentAccount.Id);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.SendMessageAsync(chatMessage.GroupId, new
                {
                    Type = "LeaveGroup",
                    chatMessage.Content,
                    MessageType = chatMessage.MessageType.ToString(),
                    chatMessage.CreatedAt,
                    SenderName = profile!.Name
                });

                return Ok(new { Message = "Left group successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("chat/messages/all/by-groupId/{groupId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetMessagesByGroupId([FromRoute] Guid groupId, [FromQuery] DateTime? beforeCreatedAt, [FromQuery] int pageSize = 10)
        {
            var group = await _chatGroupService.GetGroupByIdAsync(groupId);

            var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
            var participant = await _chatParticipantService.GetParticipantAsync(groupId, currentAccount!.Id);
            if (group == null || participant == null || participant.Status == ParticipantStatus.Left)
            {
                return NotFound(new { Message = "Group not found or you are not a participant of this group" });
            }

            var messages = await _chatMessageService.GetMessagesByGroupIdAsync(groupId, beforeCreatedAt, pageSize);

            var messageResponses = _mapper.Map<List<ChatMessageResponse>>(messages);

            // Get Sender Info 
            var userProfile = new ProfileResponse();

            foreach (var message in messageResponses)
            {
                // Map name and avatar for sender
                participant = await _chatParticipantService.GetParticipantByIdAsync(message.SenderId);
                userProfile = await _userServiceClient.GetUserProfileAsync(participant!.UserId);
                message.SenderId = participant!.UserId; // Map back to UserId
                message.SenderName = userProfile!.Name;
                message.SenderAvatar = userProfile.AvatarUrl;

                // Convert time to UTC+7
                message.CreatedAt = ConvertToUtc7(message.CreatedAt);
                if (message.EditAt.HasValue)
                {
                    message.EditAt = ConvertToUtc7(message.EditAt.Value);
                }

                foreach (var reader in message.ReaderSummary)
                {
                    userProfile = await _userServiceClient.GetUserProfileAsync(reader.UserId);
                    reader.ReaderName = userProfile!.Name;

                    // Convert time to UTC+7
                    reader.ReadAt = ConvertToUtc7(reader.ReadAt);
                }
            }

            return Ok(messageResponses);
        }

        [HttpPost("chat/send-message")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageRequest request)
        {
            try
            {
                // Get SenderId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                {
                    return new UnauthorizedResult();
                }

                var profile = await _userServiceClient.GetUserProfileAsync(currentAccount.Id);

                string? file = null;

                try
                {
                    if (request.File != null && request.File.Length > 0)
                    {
                        using var stream = request.File.OpenReadStream();
                        file = await _cloudinaryService.UploadImageAsync(stream, request.File.FileName);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"File upload failed: {ex.Message}");
                }

                var chatMessage = _mapper.Map<ChatMessage>(request);
                chatMessage.Id = Guid.NewGuid();
                chatMessage.SenderId = currentAccount!.Id; // Set SenderId
                chatMessage.CreatedAt = DateTime.UtcNow;
                chatMessage.Status = MessageStatus.Sent;
                // Map content to file url if message type is not text
                if (chatMessage.MessageType != MessageType.Text)
                {
                    chatMessage.Content = file!;
                }

                await _chatMessageService.SendMessageAsync(chatMessage);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.SendMessageAsync(request.GroupId, new
                {
                    Type = "NewMessage",
                    chatMessage.Id,
                    chatMessage.Content,
                    MessageType = chatMessage.MessageType.ToString(),
                    chatMessage.CreatedAt,
                    SenderId = chatMessage.SenderId,
                    SenderName = profile!.Name,
                    SenderAvatar = profile.AvatarUrl
                });

                return Ok(new { Message = "Message sent successfully", MessageId = chatMessage.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("chat/message/edit/{messageId:guid}")]
        [Authorize]
        public async Task<IActionResult> EditMessage([FromRoute] Guid messageId, [FromForm] EditMessageRequest request)
        {
            try
            {
                // Get EditorId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                {
                    return new UnauthorizedResult();
                }

                var message = await _chatMessageService.GetMessageByIdAsync(messageId);

                string? file = null;

                try
                {
                    if (request.File != null && request.File.Length > 0)
                    {
                        using var stream = request.File.OpenReadStream();
                        file = await _cloudinaryService.UploadImageAsync(stream, request.File.FileName);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"File upload failed: {ex.Message}");
                }

                // Check file url if message type is not text
                if (request.MessageType != MessageType.Text)
                {
                    message.Content = file!;
                }
                else
                {
                    message.Content = request.NewContent;
                }

                message.MessageType = request.MessageType;
                message.ParentMessageId = request.ParentMessageId;
                await _chatMessageService.EditMessageAsync(message);

                var profile = await _userServiceClient.GetUserProfileAsync(message.Sender.UserId);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.EditMessageAsync(message.GroupId, new
                {
                    Type = "EditMessage",
                    MessageId = messageId,
                    EditedContent = request.NewContent,
                    MessageType = request.MessageType.ToString(),
                    EditedAt = DateTime.Now,
                    EditorId = message.Sender.UserId,
                    EditorName = profile!.Name,
                    EditorAvatar = profile.AvatarUrl
                });

                return Ok(new { Message = "Message edited successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("chat/react/add")]
        [Authorize]
        public async Task<IActionResult> ReactToMessage([FromBody] CreateReactionRequest request)
        {
            try
            {
                // Get ReactorId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                    return Unauthorized();

                var reaction = _mapper.Map<MessageReaction>(request);
                reaction.Id = Guid.NewGuid();
                reaction.CreatedAt = DateTime.UtcNow;

                await _reactionService.AddReactionAsync(currentAccount.Id, reaction);

                var message = await _chatMessageService.GetMessageByIdAsync(request.MessageId);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.AddReactionAsync(message.GroupId, new
                {
                    Type = "AddReaction",
                    MessageId = request.MessageId,
                    SenderId = currentAccount.Id,
                    ReactionType = request.ReactionType.ToString(),
                    CreatedAt = ConvertToUtc7(reaction.CreatedAt)
                });
                return Ok(new { Message = "Reaction added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("chat/react/all/by-messageId/{messageId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetReactionsByMessageId([FromRoute] Guid messageId)
        {
            var reactions = await _reactionService.GetReactionsByMessageIdAsync(messageId);

            var reactionResponses = _mapper.Map<List<ReactionResponse>>(reactions);

            // Get Sender Info 
            var userProfile = new ProfileResponse();
            var participant = new ChatParticipant();
            foreach (var reaction in reactionResponses)
            {
                participant = await _chatParticipantService.GetParticipantByIdAsync(reaction.ParticipantId);
                userProfile = await _userServiceClient.GetUserProfileAsync(participant!.UserId);
                reaction.ParticipantId = participant!.UserId; // Map back to UserId
                reaction.ParticipantName = userProfile!.Name;
                reaction.ParticipantAvatar = userProfile.AvatarUrl;
            }

            return Ok(reactionResponses);
        }

        [HttpDelete("chat/react/remove/{reactionId:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveReactionFromMessage([FromRoute] Guid reactionId)
        {
            try
            {
                // Get ReactorId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                    return Unauthorized();

                var reaction = await _reactionService.RemoveReactionAsync(reactionId, currentAccount.Id);

                var message = await _chatMessageService.GetMessageByIdAsync(reaction.MessageId);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.AddReactionAsync(message.GroupId, new
                {
                    Type = "RemoveReaction",
                    MessageId = reaction.MessageId,
                    ReactionId = reaction.Id,
                    Reactiontype = reaction.ReactionType.ToString(),
                    SenderId = currentAccount.Id,
                    RemovedAt = ConvertToUtc7(DateTime.UtcNow)
                });
                return Ok(new { Message = "Reaction removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("chat/reader/add")]
        [Authorize]
        public async Task<IActionResult> AddReader([FromBody] CreateReaderRequest request)
        {
            try
            {
                // Get ReaderId By Current Account
                var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
                if (currentAccount == null)
                    return Unauthorized();

                await _readerService.MarkAsReadAsync(request.GroupId, currentAccount.Id);

                var profile = await _userServiceClient.GetUserProfileAsync(currentAccount.Id);

                // Notify to clients in group via realtime service (e.g., SignalR, WebSocket)
                await _realtimeNotifier.ReadMessageAsync(request.GroupId, new
                {
                    Type = "ReadMessage",
                    ReaderId = currentAccount.Id,
                    ReaderName = profile!.Name,
                    ReaderAvatar = profile.AvatarUrl,
                    ReadAt = ConvertToUtc7(DateTime.UtcNow)
                });
                return Ok(new { Message = "Reader added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("chat/reader/all/by-messageId/{messageId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetReadersByMessageId([FromRoute] Guid messageId)
        {
            var readers = await _readerService.GetMessageReadsByMessageIdAsync(messageId);
            var readerResponses = _mapper.Map<List<ReaderResponse>>(readers);

            // Get Reader Info 
            var userProfile = new ProfileResponse();
            var participant = new ChatParticipant();
            foreach (var reader in readerResponses)
            {
                participant = await _chatParticipantService.GetParticipantByIdAsync(reader.ReaderId);
                userProfile = await _userServiceClient.GetUserProfileAsync(participant!.UserId);
                reader.ReaderId = participant!.UserId; // Map back to UserId
                reader.ReaderName = userProfile!.Name;
                reader.ReaderAvatar = userProfile.AvatarUrl;
                // Convert time to UTC+7
                reader.ReadAt = ConvertToUtc7(reader.ReadAt);
            }
            return Ok(readerResponses);
        }

        //[HttpPost("chat/notify/add")]
        //[Authorize]
        //public async Task<IActionResult> AddNotification([FromBody] CreateNotificationRequest request)
        //{
        //    try
        //    {
        //        // Get NotifierId By Current Account
        //        var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
        //        if (currentAccount == null)
        //            return Unauthorized();
        //        var notification = _mapper.Map<Notification>(request);
        //        notification.Id = Guid.NewGuid();
        //        notification.UserId = currentAccount.Id;
        //        notification.CreatedAt = DateTime.UtcNow;
        //        await _notificationService.CreateNotificationAsync(notification);
        //        return Ok(new { Message = "Notification added successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpGet("chat/notify/all/me")]
        [Authorize]
        public async Task<IActionResult> GetMyNotifications(int amountOfNoti)
        {
            // Get UserId By Current Account
            var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
            if (currentAccount == null)
            {
                return new UnauthorizedResult();
            }
            var notifications = await _notificationService.GetMyNotificationAsync(currentAccount.Id, amountOfNoti);
            var notificationResponses = _mapper.Map<List<NotificationResponse>>(notifications);

            var message = new ChatMessage();
            var participant = new ChatParticipant();
            var senderProfile = new ProfileResponse();
            // Convert time to UTC+7
            foreach (var notification in notificationResponses)
            {
                message = await _chatMessageService.GetMessageByIdAsync(notification.ChatMessageId);
                participant = await _chatParticipantService.GetParticipantByIdAsync(message.SenderId);
                senderProfile = await _userServiceClient.GetUserProfileAsync(participant!.UserId);

                notification.SenderName = senderProfile!.Name;
                notification.MessageContent = message.Content;
                notification.CreatedAt = ConvertToUtc7(notification.CreatedAt);
            }
            return Ok(notificationResponses);
        }

        [HttpPut("chat/notify/read/{notificationId:guid}")]
        [Authorize]
        public async Task<IActionResult> ReadNotification(Guid notificationId)
        {
            var currentAccount = await _authServiceClient.GetCurrentAccountAsync();
            if (currentAccount == null) { return new UnauthorizedResult(); }

            await _notificationService.ReadNotificationAsync(notificationId, currentAccount.Id);
            return Ok(new { Message = "Read notification successfully." });
        }
    }
}
