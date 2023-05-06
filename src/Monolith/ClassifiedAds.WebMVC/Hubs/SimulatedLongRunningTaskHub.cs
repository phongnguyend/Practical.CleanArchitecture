using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.Hubs;

public class SimulatedLongRunningTaskHub : Hub
{
    public async Task SendTaskStatus(SendTaskStatusMessage message)
    {
        await Clients.All.SendAsync("ReceiveTaskStatus", $"{message.Step}  {message.Message}");
    }
}

public class SendTaskStatusMessage
{
    public string Step { get; set; }
    public string Message { get; set; }
}
