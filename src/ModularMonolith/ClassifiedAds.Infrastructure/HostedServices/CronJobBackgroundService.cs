using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HostedServices;

public abstract class CronJobBackgroundService : BackgroundService
{
    protected string Cron { get; set; }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var cron = new CronExpression(Cron);
        var next = cron.GetNextValidTimeAfter(DateTimeOffset.Now);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (DateTimeOffset.Now > next)
            {
                await DoWork(cancellationToken);

                next = cron.GetNextValidTimeAfter(DateTimeOffset.Now);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    protected abstract Task DoWork(CancellationToken cancellationToken);
}
