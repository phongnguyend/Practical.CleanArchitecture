using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ClassifiedAds.WebAPI.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
