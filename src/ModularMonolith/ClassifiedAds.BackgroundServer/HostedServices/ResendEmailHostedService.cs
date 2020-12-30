using ClassifiedAds.Modules.Notification.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices
{
    public class ResendEmailHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ResendEmailHostedService> _logger;

        public ResendEmailHostedService(IServiceProvider services,
            ILogger<ResendEmailHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ResendEmailService is starting.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ResendEmail task doing background work.");

                int rs = 0;

                using (var scope = _services.CreateScope())
                {
                    var resendEmailTask = scope.ServiceProvider.GetRequiredService<EmailMessageService>();

                    rs = resendEmailTask.ResendEmailMessages();
                }

                if (rs == 0)
                {
                    await Task.Delay(10000, stoppingToken);
                }
            }

            _logger.LogDebug($"ResendEmail background task is stopping.");
        }
    }
}