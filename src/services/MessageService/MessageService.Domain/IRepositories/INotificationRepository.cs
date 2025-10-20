using MessageService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Domain.IRepositories
{
    public interface INotificationRepository
    {
        Task SaveChangesAsync();
        Task CreateNotificationAsync(Notification notification);
        Task<List<Notification>?> GetMyNotificationAsync(Guid userId, int take);
        Task<Notification?> GetNotificationByIdAsync(Guid notificationId);
        Task BulkInsertNotificationsAsync(List<Notification> notifications);
    }
}
