using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.Web.SignalR
{
    public class SignalRNotification<T> : IWebNotification<T>
    {
        private readonly Dictionary<string, HubConnection> _connections = new Dictionary<string, HubConnection>();
        private readonly object _lock = new object();
        private readonly string _endpoint;
        private readonly string _eventName;

        public SignalRNotification(string endpoint, string hubName, string eventName)
        {
            _endpoint = endpoint + "/" + hubName;
            _eventName = eventName;
        }

        public async Task SendAsync(T message, CancellationToken cancellationToken = default)
        {
            HubConnection connection;
            lock (_lock)
            {
                if (_connections.ContainsKey(_endpoint))
                {
                    connection = _connections[_endpoint];
                }
                else
                {
                    connection = new HubConnectionBuilder()
                        .WithUrl(_endpoint)
                        .AddMessagePackProtocol()
                        .Build();
                    _connections[_endpoint] = connection;
                }

                if (connection.State != HubConnectionState.Connected)
                {
                    connection.StartAsync(cancellationToken).GetAwaiter().GetResult();
                }
            }

            await connection.InvokeAsync(_eventName, message, cancellationToken);
        }
    }
}
