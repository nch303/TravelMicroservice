using MessageService.Application.DTOs.Responses;
using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface IChatGroupService
    {
        Task CreateGroupAsync(ChatGroup chatGroup);
        Task<List<ChatGroup>> GetUserGroupsAsync(Guid userId);
        Task<ChatGroup?> GetGroupByIdAsync(Guid groupId);
        Task<ChatGroup?> GetGroupByScheduleIdAsync(Guid? scheduleId);
        Task<ShareGroupResponse> ShareGroupCode(Guid groupId, Guid onwerId);
        Task<ChatMessage> JoinGroup(string groupCode, Guid userId);
        Task<ChatMessage> LeaveGroup(Guid groupId, Guid userId);
    }
}
