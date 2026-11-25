using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Notification.InApp.SignalR;

public class SignalROptions
{
    public string Endpoint { get; set; }

    public Dictionary<string, string> Hubs { get; set; }

    public Dictionary<string, string> MethodNames { get; set; }
}
