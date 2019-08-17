using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ClassifiedAds.NotificationTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:62710/SimulatedLongRunningTaskHub")
                .AddMessagePackProtocol()
                .Build();

            connection.On<string>("ReceiveTaskStatus", (message) => { Console.WriteLine(message); });
            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Listening...");
            Console.ReadLine();
        }
    }
}
