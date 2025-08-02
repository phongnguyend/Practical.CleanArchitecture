using ClassifiedAds.Application.Users.Commands;
using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.HostedServices;

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
        _logger.LogInformation("SyncUsersWorker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var activity = ActivityExtensions.StartNew("SyncUsersWorker");

            _logger.LogDebug($"SyncUsersWorker doing background work.");

            var syncUsersCommand = new SyncUsersCommand();

            using (var scope = _services.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetDispatcher();

                await dispatcher.DispatchAsync(syncUsersCommand, stoppingToken);
            }

            if (syncUsersCommand.SyncedUsersCount == 0)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogInformation($"SyncUsersWorker task is stopping.");
    }
}