using MessageService.Application.IServices;
using MessageService.Domain.Entities;
using MessageService.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task SaveChangesAsync()
        {
            await _notificationRepository.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task<List<Notification>?> GetMyNotificationAsync(Guid userId, int take)
        {
            return await _notificationRepository.GetMyNotificationAsync(userId, take);
        }

        public async Task ReadNotificationAsync(Guid notificationId, Guid readerId)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
            if (notification == null)
            {
                throw new Exception("Notification can not be found");
            }

            if (notification.UserId != readerId)
            {
                throw new Exception("You are not the reader of this notification");
            }

            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }
    }
}
