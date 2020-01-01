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

            var rabbitMQFileUploadedEventReceiver = new RabbitMQReceiver<FileUploadedEvent>(new RabbitMQReceiverOptions
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                QueueName = "classifiedadds_fileuploaded",
            });

            rabbitMQFileUploadedEventReceiver.Receive(data =>
            {
                Console.WriteLine("RabbitMQ - File Uploaded: " + data.FileEntry.Id);
            });

            var rabbitMQFileDeletedEventReceiver = new RabbitMQReceiver<FileDeletedEvent>(new RabbitMQReceiverOptions
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                QueueName = "classifiedadds_filedeleted",
            });

            rabbitMQFileDeletedEventReceiver.Receive(data =>
            {
                Console.WriteLine("RabbitMQ - File Deleted: " + data.FileEntry.Id);
            });

            var azureQueueFileUploadedEventReceiver = new AzureQueueReceiver<FileUploadedEvent>("DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net", "classifiedadds-fileuploaded");
            azureQueueFileUploadedEventReceiver.Receive(data =>
            {
                Console.WriteLine("AzureQueue - File Uploaded:" + data.FileEntry.Id);
            });

            var azureQueueFileDeletedEventReceiver = new AzureQueueReceiver<FileDeletedEvent>("DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net", "classifiedadds-filedeleted");
            azureQueueFileDeletedEventReceiver.Receive(data =>
            {
                Console.WriteLine("AzureQueue - File Deleted:" + data.FileEntry.Id);
            });

            var azureServiceBusFileUploadedEventReceiver = new AzureServiceBusReceiver<FileUploadedEvent>("Endpoint=sb://xxx.servicebus.windows.net/;SharedAccessKeyName=xxx;SharedAccessKey=xxx", "classifiedadds_fileuploaded");
            azureServiceBusFileUploadedEventReceiver.Receive(data =>
            {
                Console.WriteLine("AzureServiceBus - File Uploaded:" + data.FileEntry.Id);
            });

            var azureServiceBusFileDeletedEventReceiver = new AzureServiceBusReceiver<FileUploadedEvent>("Endpoint=sb://xxx.servicebus.windows.net/;SharedAccessKeyName=xxx;SharedAccessKey=xxx", "classifiedadds_filedeleted");
            azureServiceBusFileDeletedEventReceiver.Receive(data =>
            {
                Console.WriteLine("AzureServiceBus - File Deleted:" + data.FileEntry.Id);
            });

            Console.WriteLine("Listening...");
            Console.ReadLine();

            rabbitMQFileUploadedEventReceiver.Dispose();
            rabbitMQFileDeletedEventReceiver.Dispose();
        }
    }
}
