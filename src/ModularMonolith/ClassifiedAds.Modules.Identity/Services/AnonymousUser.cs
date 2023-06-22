using ClassifiedAds.Contracts.Identity.Services;
using System;

namespace ClassifiedAds.Modules.Identity.Services;

public class AnonymousUser : ICurrentUser
{
    public bool IsAuthenticated => false;

    public Guid UserId => Guid.Empty;
}
