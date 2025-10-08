using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.IRepositories
{
    public interface IChatParticipantRepository
    {
        Task AddParticipantAsync(ChatParticipant chatParticipant);
        Task<ChatParticipant?> GetParticipantAsync(Guid chatGroupId, Guid userId);
        Task<ChatParticipant?> GetParticipantByIdAsync(Guid participantId);
        Task SaveChangesAsync();
        Task<List<ChatParticipant>> GetActiveParticipantsByGroupIdAsync(Guid chatGroupId);

    }
}
