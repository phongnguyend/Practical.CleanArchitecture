using ClassifiedAds.Application.EmailMessages.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices
{
    public class SendEmailHostedService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SendEmailHostedService> _logger;

        public SendEmailHostedService(IServiceProvider services,
            ILogger<SendEmailHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("SendEmailService is starting.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"SendEmail task doing background work.");

                int rs = 0;

                using (var scope = _services.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailMessageService>();

                    rs = await emailService.SendEmailMessagesAsync();
                }

                if (rs == 0)
                {
                    await Task.Delay(10000, stoppingToken);
                }
            }

            _logger.LogDebug($"SendEmail background task is stopping.");
        }
    }
}