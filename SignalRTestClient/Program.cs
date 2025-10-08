using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var groupId = "63ab164c-e350-4897-8383-3557db45d109";

        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7022/chatHub", options =>
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
        connection.On<JsonElement>("ReceiveMessage", (message) =>
        {
            Console.WriteLine("📩 Raw message JSON:");
            Console.WriteLine(message.ToString());
        });

        connection.On<JsonElement>("ReceiveJoinGroupEvent", (joinerInfo) =>
        {
            Console.WriteLine("👥 New participant joined the group:");
            Console.WriteLine(joinerInfo.ToString());
        });

        await connection.StartAsync();
        Console.WriteLine("✅ Connected to ChatHub.");

        await connection.InvokeAsync("JoinGroup", groupId);
        Console.WriteLine($"👥 Joined group: {groupId}");

        Console.WriteLine("📡 Listening for realtime messages...");
        Console.ReadLine();

        await connection.StopAsync();
    }
}
