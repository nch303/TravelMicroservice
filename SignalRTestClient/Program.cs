using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        //var groupId = "63ab164c-e350-4897-8383-3557db45d109";

        //var connection = new HubConnectionBuilder()
        //    .WithUrl("https://localhost:7022/chatHub", options =>
        //    {
        //        options.HttpMessageHandlerFactory = _ => new HttpClientHandler
        //        {
        //            ServerCertificateCustomValidationCallback =
        //                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //        };
        //    })
        //    .WithAutomaticReconnect()
        //    .Build();

        //// 👇 Đăng ký event TRƯỚC khi kết nối
        //connection.On<JsonElement>("ReceiveMessage", (message) =>
        //{
        //    Console.WriteLine("📩 A message was sent");
        //    Console.WriteLine(message.ToString());
        //});

        //connection.On<JsonElement>("EditMessage", (message) =>
        //{
        //    Console.WriteLine("📩 A message was edited");
        //    Console.WriteLine(message.ToString());
        //});

        //connection.On<JsonElement>("AddReaction", (reactionInfo) =>
        //{
        //    Console.WriteLine("📩 A reaction was added");
        //    Console.WriteLine(reactionInfo.ToString());
        //});

        //connection.On<JsonElement>("RemoveReaction", (reactionInfo) =>
        //{
        //    Console.WriteLine("📩 A reaction was removed.");
        //    Console.WriteLine(reactionInfo.ToString());
        //});

        //connection.On<JsonElement>("ReadMessage", (readInfo) =>
        //{
        //    Console.WriteLine("📩 A message was read.");
        //    Console.WriteLine(readInfo.ToString());
        //});

        //await connection.StartAsync();
        //Console.WriteLine("✅ Connected to ChatHub.");

        //await connection.InvokeAsync("JoinGroup", groupId);
        //Console.WriteLine($"👥 Joined group: {groupId}");

        //Console.WriteLine("📡 Listening for realtime messages...");
        //Console.ReadLine();

        // Notification
        var connection2 = new HubConnectionBuilder()
            .WithUrl("https://localhost:7021/notificationHub", options =>
            {
                options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .WithAutomaticReconnect()
            .Build();

        // 👇 Đăng ký event TRƯỚC khi kết nối
        connection2.On<JsonElement>("ReceiveNotification", (notification) =>
        {
            Console.WriteLine("📩 A notification was sent");
            Console.WriteLine(notification.ToString());
        });

        await connection2.StartAsync();
        Console.WriteLine("✅ Connected to NotiHub.");

        var currentUserId = "5F44A969-8518-4863-8B00-B6170F1B4311".ToLower();
        var currentScheduleId = "C1F31D44-3D35-453A-3A4C-08DE00029182".ToLower();

        // Nếu user đang trong 1 lịch trình
        await connection2.InvokeAsync("JoinUser", currentUserId);
        Console.WriteLine($"{currentUserId}");
     
        // Tham gia nhóm cá nhân
        await connection2.InvokeAsync("JoinSchedule", currentScheduleId);
        Console.WriteLine($"{currentScheduleId}");

        Console.WriteLine("📡 Listening for realtime messages...");
        Console.ReadLine();

        //await connection.StopAsync();
        await connection2.StopAsync();
    }
}
