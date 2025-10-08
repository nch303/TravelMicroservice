using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.IServices
{
    public interface INotificationService
    {
        Task SaveChangesAsync();
        Task CreateNotificationAsync(Notification notification);
        Task<List<Notification>?> GetMyNotificationAsync(Guid userId, int take);
        Task ReadNotificationAsync(Guid notification, Guid readerId);
    }
}
