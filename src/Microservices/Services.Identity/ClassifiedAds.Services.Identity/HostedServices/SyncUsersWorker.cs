using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Commands.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.HostedServices;

public class SyncUsersWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SyncUsersWorker> _logger;

    public SyncUsersWorker(IServiceProvider services,
        ILogger<SyncUsersWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("SyncUsersWorker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"SyncUsersWorker doing background work.");

            var syncUsersCommand = new SyncUsersCommand();

            using (var scope = _services.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();

                await dispatcher.DispatchAsync(syncUsersCommand, stoppingToken);
            }

            if (syncUsersCommand.SyncedUsersCount == 0)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogDebug($"SyncUsersWorker task is stopping.");
    }
}