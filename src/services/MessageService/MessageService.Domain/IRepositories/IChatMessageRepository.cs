using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.IRepositories
{
    public interface IChatMessageRepository
    {
        Task SendMessageAsync(ChatMessage chatMessage);
        Task SaveChangeAsync();
        Task<ChatMessage?> GetMessageByIdAsync(Guid messageId);
        Task<List<ChatMessage>> GetMessagesByGroupIdAsync(Guid groupId, DateTime? beforeCreatedAt, int pageSize);
    }
}
