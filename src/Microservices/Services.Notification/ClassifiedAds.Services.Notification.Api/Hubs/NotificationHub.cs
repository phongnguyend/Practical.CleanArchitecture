using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ClassifiedAds.Services.Notification.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
