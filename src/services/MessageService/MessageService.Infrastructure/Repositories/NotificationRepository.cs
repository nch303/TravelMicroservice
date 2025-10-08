using MessageService.Domain.Entities;
using MessageService.Domain.Enums;
using MessageService.Domain.IRepositories;
using MessageService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>?> GetMyNotificationAsync(Guid userId, int take = 15)
        {
            // Lấy notification cơ bản từ DB
            var notifications = await _context.Notifications
                .Include(n => n.ChatGroup)
                .Where(n => n.UserId == userId && n.ChatGroupId != null 
                         && n.ChatGroup.Participants.FirstOrDefault(p => p.UserId == userId)!.Status != ParticipantStatus.Left)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(); // => đây chỉ query DB, chưa group

            // Xử lý GroupBy và lấy notification mới nhất mỗi nhóm ở client
            var latestPerGroup = notifications
                .GroupBy(n => n.ChatGroupId)
                .Select(g => g.First()) // đã orderByDescending từ DB
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToList();

            return latestPerGroup;
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid notificationId)
        {
            return await _context.Notifications
                .Include(n => n.ChatGroup)
                .FirstOrDefaultAsync(n => n.Id == notificationId);
        }

        public async Task BulkInsertNotificationsAsync(List<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

    }
}
