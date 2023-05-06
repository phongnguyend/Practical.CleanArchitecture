using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ClassifiedAds.WebMVC.Hubs;

[Authorize]
public class AuthorizedHub : Hub
{
}
