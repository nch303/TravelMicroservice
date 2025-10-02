using AutoMapper;
using MessageService.Application.DTOs.Requests;
using MessageService.Application.DTOs.Responses;
using MessageService.Application.IServiceClients;
using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public ChatGroupController(IChatGroupService chatGroupService, IMapper mapper, IAuthServiceClient authServiceClient
            , IChatMessageService chatMessageService, ICloudinaryService cloudinaryService
            , IChatParticipantService chatParticipantService)
        {
            _chatGroupService = chatGroupService;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _chatMessageService = chatMessageService;
            _cloudinaryService = cloudinaryService;
            _chatParticipantService = chatParticipantService;
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
                return Ok(new { Message = "Message sent successfully", MessageId = chatMessage.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("chat/messages/group/{groupId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetMessagesByGroupId([FromRoute] Guid groupId, [FromQuery] DateTime? beforeCreatedAt, [FromQuery] int pageSize = 10)
        {
            var messages = await _chatMessageService.GetMessagesByGroupIdAsync(groupId, beforeCreatedAt, pageSize);
            var messageResponses = _mapper.Map<List<ChatMessageResponse>>(messages);
            return Ok(messageResponses);
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
                return Ok(new { Message = "Message edited successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
