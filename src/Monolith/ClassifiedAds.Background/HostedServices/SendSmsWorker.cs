using ClassifiedAds.Application.SmsMessages.Commands;
using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.HostedServices;

public class SendSmsWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<SendSmsWorker> _logger;

    public SendSmsWorker(IServiceProvider services,
        ILogger<SendSmsWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SendSmsService is starting.");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var activity = ActivityExtensions.StartNew("SendSmsWorker");

            _logger.LogDebug($"SendSms task doing background work.");

            try
            {
                var sendSmsesCommand = new SendSmsMessagesCommand();

                using (var scope = _services.CreateScope())
                {
                    var dispatcher = scope.ServiceProvider.GetDispatcher();

                    await dispatcher.DispatchAsync(sendSmsesCommand, stoppingToken);
                }

                if (sendSmsesCommand.SentMessagesCount == 0)
                {
                    await Task.Delay(10000, stoppingToken);
                }
            }
            catch (CircuitBreakerOpenException)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogInformation($"ResendSms background task is stopping.");
    }
}