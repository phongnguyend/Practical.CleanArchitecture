using ClassifiedAds.Application;
using ClassifiedAds.Application.SmsMessages.Commands;
using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.HostedServices;

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
        _logger.LogDebug("SendSmsService is starting.");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"SendSms task doing background work.");

            try
            {
                var sendSmsesCommand = new SendSmsMessagesCommand();

                using (var scope = _services.CreateScope())
                {
                    var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();

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

        _logger.LogDebug($"ResendSms background task is stopping.");
    }
}