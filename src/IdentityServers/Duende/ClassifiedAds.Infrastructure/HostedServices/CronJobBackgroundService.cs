using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HostedServices;

public abstract class CronJobBackgroundService : BackgroundService
{
    protected string Cron { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cron = new CronExpression(Cron);
        var next = cron.GetNextValidTimeAfter(DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTimeOffset.Now > next)
            {
                await DoWork(stoppingToken);

                next = cron.GetNextValidTimeAfter(DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    protected abstract Task DoWork(CancellationToken stoppingToken);
}
