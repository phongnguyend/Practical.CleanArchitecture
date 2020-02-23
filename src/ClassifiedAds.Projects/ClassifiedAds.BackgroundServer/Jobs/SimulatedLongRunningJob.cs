using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.Jobs
{
    public class SimulatedLongRunningJob
    {
        private readonly IConfiguration _configuration;

        public SimulatedLongRunningJob(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"{_configuration["NotificationServer:Endpoint"]}/SimulatedLongRunningTaskHub")
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
