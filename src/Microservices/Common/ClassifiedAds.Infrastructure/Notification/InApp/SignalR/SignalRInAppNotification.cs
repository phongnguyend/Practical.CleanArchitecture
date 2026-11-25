using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.InApp.SignalR;

public class SignalRInAppNotification<T> : IInAppNotification<T>
{
    private readonly HubConnection _connection;
    private readonly string _endpoint;
    private readonly string _eventName;

    public SignalRInAppNotification(string endpoint, string hubName, string eventName)
    {
        _endpoint = endpoint + "/" + hubName;
        _eventName = eventName;

        _connection = new HubConnectionBuilder()
                    .WithUrl(_endpoint)
                    .AddMessagePackProtocol()
                    .Build();
    }

    public async Task SendAsync(T message, CancellationToken cancellationToken = default)
    {
        if (_connection.State != HubConnectionState.Connected)
        {
            _connection.StartAsync(cancellationToken).GetAwaiter().GetResult();
        }

        await _connection.InvokeAsync(_eventName, message, cancellationToken);
    }
}
