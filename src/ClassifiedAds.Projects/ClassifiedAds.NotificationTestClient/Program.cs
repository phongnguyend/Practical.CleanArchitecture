using ClassifiedAds.DomainServices.DomainEvents;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ClassifiedAds.NotificationTestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:62710/SimulatedLongRunningTaskHub")
                .AddMessagePackProtocol()
                .Build();

            connection.On<string>("ReceiveTaskStatus", (message) => { Console.WriteLine(message); });
            connection.StartAsync().GetAwaiter().GetResult();

            RabbitMQReceiver rabbitMQReceiver = new RabbitMQReceiver(new RabbitMQReceiverOptions
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                QueueName = "classifiedadds",
            });

            rabbitMQReceiver.Receive<FileUploadedEvent>(data =>
            {
                Console.WriteLine(data.FileEntry.Id);
            });

            AzureQueueReceiver azureQueueReceiver = new AzureQueueReceiver("DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net", "classifiedadds");
            azureQueueReceiver.Receive<FileUploadedEvent>(data =>
            {
                Console.WriteLine(data.FileEntry.Id);
            });

            AzureServiceBusReceiver azureServiceBusReceiver = new AzureServiceBusReceiver("Endpoint=sb://xxx.servicebus.windows.net/;SharedAccessKeyName=xxx;SharedAccessKey=xxx", "classifiedadds");
            azureServiceBusReceiver.Receive<FileUploadedEvent>(data =>
            {
                Console.WriteLine(data.FileEntry.Id);
            });

            Console.WriteLine("Listening...");
            Console.ReadLine();

            rabbitMQReceiver.Dispose();
        }
    }
}
