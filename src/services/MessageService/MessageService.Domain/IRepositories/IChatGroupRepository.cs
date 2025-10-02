using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.IRepositories
{
    public interface IChatGroupRepository
    {
        Task CreateGroupAsync(ChatGroup chatGroup);
        Task<List<ChatGroup>> GetUserGroupsAsync(Guid userId);
        Task<ChatGroup?> GetGroupByIdAsync(Guid groupId);
        Task<ChatGroup?> GetGroupByScheduleIdAsync(Guid? scheduleId);
    }
}
