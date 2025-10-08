using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface IReaderService
    {
        Task MarkAsReadAsync(Guid groupId, Guid userId);
        Task<List<ChatMessage>?> GetUnreadMessagesAsync(Guid groupId, Guid userId);
        Task<List<MessageRead>> GetMessageReadsByMessageIdAsync(Guid messageId);
    }
}
