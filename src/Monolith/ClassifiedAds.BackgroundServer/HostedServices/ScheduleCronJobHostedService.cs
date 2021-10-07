using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices
{
    public class ScheduleCronJobHostedService : CronJobBackgroundService
    {
        private readonly ILogger<ScheduleCronJobHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ScheduleCronJobHostedService(ILogger<ScheduleCronJobHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            Cron = "0 0/1 * 1/1 * ? *"; // every minute
        }

        protected override async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

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
}
