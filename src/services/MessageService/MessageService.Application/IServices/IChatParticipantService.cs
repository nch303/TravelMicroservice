using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface IChatParticipantService
    {
        Task AddParticipantAsync(ChatParticipant chatParticipant);
        Task<ChatParticipant?> GetParticipantAsync(Guid chatGroupId, Guid participantId);
    }
}
