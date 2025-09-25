using ClassifiedAds.Application.EventLogs.Commands;
using ClassifiedAds.Application.FeatureToggles;
using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.HostedServices;

public class PublishEventWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IOutboxPublishingToggle _outboxPublishingToggle;
    private readonly ILogger<PublishEventWorker> _logger;

    public PublishEventWorker(IServiceProvider services,
        IOutboxPublishingToggle outboxPublishingToggle,
        ILogger<PublishEventWorker> logger)
    {
        _services = services;
        _outboxPublishingToggle = outboxPublishingToggle;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PushlishEventWorker is starting.");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_outboxPublishingToggle.IsEnabled())
            {
                _logger.LogInformation("PushlishEventWorker is being paused. Retry in 10s.");
                await Task.Delay(10000, stoppingToken);
                continue;
            }

            using var activity = ActivityExtensions.StartNew("PublishEventWorker");

            _logger.LogDebug($"PushlishEvent task doing background work.");

            try
            {
                var publishEventsCommand = new PublishEventsCommand();

                using (var scope = _services.CreateScope())
                {
                    var dispatcher = scope.ServiceProvider.GetDispatcher();

                    await dispatcher.DispatchAsync(publishEventsCommand, stoppingToken);
                }

                if (publishEventsCommand.SentEventsCount == 0)
                {
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (CircuitBreakerOpenException)
            {
                await Task.Delay(10000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"");
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogInformation($"PushlishEventWorker background task is stopping.");
    }
}
