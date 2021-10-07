using ClassifiedAds.Application.SmsMessages.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices
{
    public class SendSmsHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SendSmsHostedService> _logger;

        public SendSmsHostedService(IServiceProvider services,
            ILogger<SendSmsHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("SendSmsService is starting.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"SendSms task doing background work.");

                int rs = 0;

                using (var scope = _services.CreateScope())
                {
                    var smsService = scope.ServiceProvider.GetRequiredService<SmsMessageService>();

                    rs = await smsService.SendSmsMessagesAsync();
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