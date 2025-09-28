using ClassifiedAds.Application.FeatureToggles;
using ClassifiedAds.Modules.Storage.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.HostedServices;

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

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("PublishEventWorker is starting.");
        await DoWork(cancellationToken);
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_outboxPublishingToggle.IsEnabled())
            {
                _logger.LogInformation("PushlishEventWorker is being paused. Retry in 10s.");
                await Task.Delay(10000, cancellationToken);
                continue;
            }

            _logger.LogDebug($"PublishEventWorker task doing background work.");

            try
            {
                var publishEventsCommand = new PublishEventsCommand();

                using (var scope = _services.CreateScope())
                {
                    var dispatcher = scope.ServiceProvider.GetDispatcher();

                    await dispatcher.DispatchAsync(publishEventsCommand, cancellationToken);
                }

                if (publishEventsCommand.SentEventsCount == 0)
                {
                    await Task.Delay(10000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"");
                await Task.Delay(10000, cancellationToken);
            }
        }

        _logger.LogDebug($"PublishEventWorker background task is stopping.");
    }
}
