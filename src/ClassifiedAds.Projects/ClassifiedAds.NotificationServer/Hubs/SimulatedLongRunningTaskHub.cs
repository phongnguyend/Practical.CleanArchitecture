using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ClassifiedAds.NotificationServer.Hubs
{
    public class SendTaskStatusMessage
    {
        public string Step { get; set; }
        public string Message { get; set; }
    }

    public class SimulatedLongRunningTaskHub : Hub
    {
        public async Task SendTaskStatus(SendTaskStatusMessage message)
        {
            await Clients.All.SendAsync("ReceiveTaskStatus", $"{message.Step}  {message.Message}");
        }
    }
}
