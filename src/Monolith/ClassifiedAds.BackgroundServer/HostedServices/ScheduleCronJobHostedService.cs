using ClassifiedAds.Application.BackgroundTasks;
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
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<SimulatedLongRunningJob>();

            await job.Run();
        }
    }
}
