using ClassifiedAds.Infrastructure.Identity;
using System;

namespace ClassifiedAds.BackgroundServer.Identity;

public class CurrentUser : ICurrentUser
{
    public bool IsAuthenticated => false;

    public Guid UserId => Guid.Empty;
}
