using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub
{
    public class AzureEventHubReceiver<T> : IMessageReceiver<T>, IDisposable
    {
        private readonly string _eventHubConnectionString;
        private readonly string _eventHubName;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;

        public AzureEventHubReceiver(string eventHubConnectionString, string eventHubName, string storageConnectionString, string storageContainerName)
        {
            _eventHubConnectionString = eventHubConnectionString;
            _eventHubName = eventHubName;
            _storageConnectionString = storageConnectionString;
            _storageContainerName = storageContainerName;
        }

        public void Dispose()
        {
        }

        public void Receive(Action<T, MetaData> action)
        {
            ReceiveAsync(action).GetAwaiter().GetResult();
        }

        public async Task ReceiveAsync(Action<T, MetaData> action)
        {
            var storageClient = new BlobContainerClient(_storageConnectionString, _storageContainerName);

            Task ProcessEventHandler(ProcessEventArgs eventArgs)
            {
                try
                {
                    var messageAsString = Encoding.UTF8.GetString(eventArgs.Data.EventBody);
                    var message = JsonSerializer.Deserialize<Message<T>>(messageAsString);
                    action(message.Data, message.MetaData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return Task.CompletedTask;
            }

            Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
            {
                try
                {
                    Console.WriteLine(eventArgs.Exception);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return Task.CompletedTask;
            }

            var processor = new EventProcessorClient(
                storageClient,
                EventHubConsumerClient.DefaultConsumerGroupName,
                _eventHubConnectionString,
                _eventHubName);

            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;
            await processor.StartProcessingAsync();
        }
    }
}
