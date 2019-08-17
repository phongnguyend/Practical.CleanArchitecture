using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServices.Jobs
{
    public class SimulatedLongRunningJob
    {
        public async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:62710/SimulatedLongRunningTaskHub")
                .AddMessagePackProtocol()
                .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("SendTaskStatus", "Step 1", "Begining xxx");

            Thread.Sleep(2000);

            await connection.InvokeAsync("SendTaskStatus", "Step 1", "Finished xxx");

            await connection.StopAsync();
        }
    }
}
