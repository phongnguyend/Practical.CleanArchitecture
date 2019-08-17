using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ClassifiedAds.NotificationServer.Hubs
{
    public class SimulatedLongRunningTaskHub : Hub
    {
        public async Task SendTaskStatus(string step, string message)
        {
            await Clients.All.SendAsync("ReceiveTaskStatus", $"{step}  {message}");
        }
    }
}
