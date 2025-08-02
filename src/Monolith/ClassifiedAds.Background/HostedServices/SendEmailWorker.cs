using ClassifiedAds.Application.EmailMessages.Commands;
using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.HostedServices;

public class SendEmailWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SendEmailWorker> _logger;

    public SendEmailWorker(IServiceProvider services,
        ILogger<SendEmailWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SendEmailService is starting.");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var activity = ActivityExtensions.StartNew("SendEmailWorker");

            _logger.LogDebug($"SendEmail task doing background work.");

            try
            {
                var sendEmailsCommand = new SendEmailMessagesCommand();

                using (var scope = _services.CreateScope())
                {
                    var dispatcher = scope.ServiceProvider.GetDispatcher();

                    await dispatcher.DispatchAsync(sendEmailsCommand, stoppingToken);
                }

                if (sendEmailsCommand.SentMessagesCount == 0)
                {
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (CircuitBreakerOpenException)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogInformation($"SendEmail background task is stopping.");
    }
}