using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub
{
    public class AzureEventHubReceiver<T> : IMessageReceiver<T>, IDisposable
    {
        private readonly string _eventHubConnectionString;
        private readonly string _eventHubName;
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;
        private EventProcessorHost _eventProcessorHost;

        public AzureEventHubReceiver(string eventHubConnectionString, string eventHubName, string storageConnectionString, string storageContainerName)
        {
            _eventHubConnectionString = eventHubConnectionString;
            _eventHubName = eventHubName;
            _storageConnectionString = storageConnectionString;
            _storageContainerName = storageContainerName;
        }

        public void Dispose()
        {
            if (_eventProcessorHost != null)
            {
                _eventProcessorHost.UnregisterEventProcessorAsync().GetAwaiter().GetResult();
            }
        }

        public void Receive(Action<T> action)
        {
            ReceiveAsync(action).GetAwaiter().GetResult();
        }

        public async Task ReceiveAsync(Action<T> action)
        {
            _eventProcessorHost = new EventProcessorHost(
                             _eventHubName,
                             PartitionReceiver.DefaultConsumerGroupName,
                             _eventHubConnectionString,
                             _storageConnectionString,
                             _storageContainerName);

            await _eventProcessorHost.RegisterEventProcessorFactoryAsync(new EventProcessorFactory<T>(action));
        }
    }
}
