using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.IServices
{
    public interface INotificationRecipientService
    {
        Task UpdateNoticationRecipientAsync(Guid notificationRecipientId);
        Task<List<NotificationRecipient>> GetAllNotificationRecipientsByUserIdAsync(Guid userId);
    }
}
