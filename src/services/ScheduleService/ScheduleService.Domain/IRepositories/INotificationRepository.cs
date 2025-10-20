using ScheduleService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Domain.IRepositories
{
    public interface INotificationRepository
    {
        Task SaveChangesAsync();
        Task CreateNotificationAsync(Notification notification);
    }
}
