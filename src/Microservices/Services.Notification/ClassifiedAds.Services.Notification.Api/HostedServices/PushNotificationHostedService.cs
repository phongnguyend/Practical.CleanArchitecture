using ClassifiedAds.Services.Notification.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Notification.HostedServices;

public class PushNotificationHostedService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<PushNotificationHostedService> _logger;

    public PushNotificationHostedService(IServiceProvider services,
        ILogger<PushNotificationHostedService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var notificationHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                await notificationHubContext.Clients.All.SendAsync("ReceiveMessage", $"Test message from NotificationHub ...", cancellationToken: stoppingToken);
            }

            await Task.Delay(30000, stoppingToken);
        }
    }
}