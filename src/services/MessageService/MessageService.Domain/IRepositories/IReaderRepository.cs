using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.IRepositories
{
    public interface IReaderRepository
    {
        Task MarkAsReadAsync(MessageRead messageRead);
        Task<List<ChatMessage>?> GetUnreadMessagesAsync(Guid groupId, Guid readerId);
        Task<List<MessageRead>> GetMessageReadsByMessageIdAsync(Guid messageId);
    }
}
