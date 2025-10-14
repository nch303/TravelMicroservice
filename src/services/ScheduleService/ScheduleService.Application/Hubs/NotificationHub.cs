using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleService.Application.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinGroup(string scheduleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, scheduleId);
            Console.WriteLine($"Client {Context.ConnectionId} joined group {scheduleId}");
        }

        public async Task LeaveGroup(string scheduleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, scheduleId);
            Console.WriteLine($"Client {Context.ConnectionId} left group {scheduleId}");
        }

        // ✅ Khi 1 client kết nối vào
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // ✅ Khi 1 client ngắt kết nối
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
