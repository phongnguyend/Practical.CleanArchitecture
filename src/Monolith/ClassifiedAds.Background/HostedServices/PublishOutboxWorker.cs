using ClassifiedAds.Application.FeatureToggles;
using ClassifiedAds.Application.OutboxMessages.Commands;
using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.HostedServices;

public class PublishOutboxWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IOutboxPublishingToggle _outboxPublishingToggle;
    private readonly ILogger<PublishOutboxWorker> _logger;

    public PublishOutboxWorker(IServiceProvider services,
        IOutboxPublishingToggle outboxPublishingToggle,
        ILogger<PublishOutboxWorker> logger)
    {
        _services = services;
        _outboxPublishingToggle = outboxPublishingToggle;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PublishOutboxWorker is starting.");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_outboxPublishingToggle.IsEnabled())
            {
                _logger.LogInformation("PublishOutboxWorker is being paused. Retry in 10s.");
                await Task.Delay(10000, stoppingToken);
                continue;
            }

            using var activity = ActivityExtensions.StartNew("PublishOutboxWorker");

            _logger.LogDebug($"PublishOutboxWorker task doing background work.");

            try
            {
                var publishEventsCommand = new PublishOutboxMessagesCommand();

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

        _logger.LogInformation($"PublishOutboxWorker background task is stopping.");
    }
}
