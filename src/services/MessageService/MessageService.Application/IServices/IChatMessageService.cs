using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface IChatMessageService
    {
        Task SendMessageAsync(ChatMessage chatMessage);
        Task EditMessageAsync(ChatMessage chatMessage);
        Task<List<ChatMessage>> GetMessagesByGroupIdAsync(Guid groupId, DateTime? beforeCreatedAt, int pageSize);
        Task<ChatMessage?> GetMessageByIdAsync(Guid messageId);
    }
}
