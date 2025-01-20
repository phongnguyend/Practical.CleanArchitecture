using ClassifiedAds.CrossCuttingConcerns.Logging;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.HostedServices;

public class ScheduleCronJobWorker : CronJobBackgroundService
{
    private readonly ILogger<ScheduleCronJobWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ScheduleCronJobWorker(ILogger<ScheduleCronJobWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        Cron = "0 0/1 * 1/1 * ? *"; // every minute
    }

    protected override async Task DoWork(CancellationToken stoppingToken)
    {
        using var activity = ActivityExtensions.StartNew("ScheduleCronJobWorker");

        try
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            var notification = scope.ServiceProvider.GetRequiredService<IWebNotification<SendTaskStatusMessage>>();

            await notification.SendAsync(new SendTaskStatusMessage { Step = "Step 1", Message = "Begining xxx" }, stoppingToken);

            await Task.Delay(2000, stoppingToken);

            await notification.SendAsync(new SendTaskStatusMessage { Step = "Step 1", Message = "Finished xxx" }, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, string.Empty);
        }
    }
}

public class SendTaskStatusMessage
{
    public string Step { get; set; }
    public string Message { get; set; }
}
