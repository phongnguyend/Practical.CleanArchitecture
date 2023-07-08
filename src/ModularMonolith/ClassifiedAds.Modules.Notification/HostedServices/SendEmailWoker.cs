using ClassifiedAds.Application;
using ClassifiedAds.Modules.Notification.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.HostedServices;

public class SendEmailWoker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SendEmailWoker> _logger;

    public SendEmailWoker(IServiceProvider services,
        ILogger<SendEmailWoker> logger)
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

            var sendEmailsCommand = new SendEmailMessagesCommand();

            using (var scope = _services.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();

                await dispatcher.DispatchAsync(sendEmailsCommand);
            }

            if (sendEmailsCommand.SentMessagesCount == 0)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogDebug($"SendEmail background task is stopping.");
    }
}