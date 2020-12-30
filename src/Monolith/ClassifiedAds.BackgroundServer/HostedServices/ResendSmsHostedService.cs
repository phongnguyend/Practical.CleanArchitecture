using ClassifiedAds.Application.SmsMessages.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices
{
    public class ResendSmsHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ResendSmsHostedService> _logger;

        public ResendSmsHostedService(IServiceProvider services,
            ILogger<ResendSmsHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ResendSmsService is starting.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ResendSms task doing background work.");

                int rs = 0;

                using (var scope = _services.CreateScope())
                {
                    var resendSmsTask = scope.ServiceProvider.GetRequiredService<SmsMessageService>();

                    rs = resendSmsTask.ResendSmsMessage();
                }

                if (rs == 0)
                {
                    await Task.Delay(10000, stoppingToken);
                }
            }

            _logger.LogDebug($"ResendSms background task is stopping.");
        }
    }
}