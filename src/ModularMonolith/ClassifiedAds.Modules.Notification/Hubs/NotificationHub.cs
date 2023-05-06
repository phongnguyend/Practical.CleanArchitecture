using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ClassifiedAds.Modules.Notification.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
