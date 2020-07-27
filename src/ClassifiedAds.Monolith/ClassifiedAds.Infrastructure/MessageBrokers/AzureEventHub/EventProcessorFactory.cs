using Microsoft.Azure.EventHubs.Processor;
using System;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub
{
    public class EventProcessorFactory<T> : IEventProcessorFactory
    {
        private readonly Action<T> _action;

        public EventProcessorFactory(Action<T> action)
        {
            _action = action;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor<T>(_action);
        }
    }
}
