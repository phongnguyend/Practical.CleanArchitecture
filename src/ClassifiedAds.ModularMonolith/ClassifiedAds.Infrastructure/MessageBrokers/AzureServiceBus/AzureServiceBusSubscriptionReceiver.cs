using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus
{
    public class AzureServiceBusSubscriptionReceiver<T> : IMessageReceiver<T>
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly SubscriptionClient _subscriptionClient;

        public AzureServiceBusSubscriptionReceiver(string connectionString, string topicName, string subscriptionName)
        {
            _connectionString = connectionString;
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            _subscriptionClient = new SubscriptionClient(_connectionString, _topicName, _subscriptionName);
        }

        public void Receive(Action<T> action)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false,
            };

            _subscriptionClient.RegisterMessageHandler((Message message, CancellationToken token) =>
            {
                var data = Encoding.UTF8.GetString(message.Body);
                action(JsonConvert.DeserializeObject<T>(data));
                return _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }, messageHandlerOptions);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
