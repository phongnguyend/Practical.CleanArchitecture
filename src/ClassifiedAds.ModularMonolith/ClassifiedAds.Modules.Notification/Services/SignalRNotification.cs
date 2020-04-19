using ClassifiedAds.Modules.Notification.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Services
{
    public class SignalRNotification : IWebNotification
    {
        private readonly Dictionary<string, HubConnection> _connections = new Dictionary<string, HubConnection>();
        private readonly object _lock = new object();

        public SignalRNotification()
        {
        }

        public void Send<T>(string endpoint, string eventName, T message)
        {
            SendAsync(endpoint, eventName, message).GetAwaiter().GetResult();
        }

        public async Task SendAsync<T>(string endpoint, string eventName, T message)
        {
            HubConnection connection;
            lock (_lock)
            {
                if (_connections.ContainsKey(endpoint))
                {
                    connection = _connections[endpoint];
                }
                else
                {
                    connection = new HubConnectionBuilder()
                        .WithUrl(endpoint)
                        .AddMessagePackProtocol()
                        .Build();
                    _connections[endpoint] = connection;
                }

                if (connection.State != HubConnectionState.Connected)
                {
                    connection.StartAsync().GetAwaiter().GetResult();
                }
            }

            await connection.InvokeAsync(eventName, message);
        }
    }
}
