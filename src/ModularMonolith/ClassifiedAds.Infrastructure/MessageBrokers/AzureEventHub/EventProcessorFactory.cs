using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.EventHubs.Processor;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub
{
    public class EventProcessorFactory<T> : IEventProcessorFactory
    {
        private readonly Action<T, MetaData> _action;

        public EventProcessorFactory(Action<T, MetaData> action)
        {
            _action = action;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor<T>(_action);
        }
    }
}
