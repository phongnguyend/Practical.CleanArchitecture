using ClassifiedAds.Domain.Identity;
using System;

namespace ClassifiedAds.Infrastructure.Identity;

public class AnonymousUser : ICurrentUser
{
    public bool IsAuthenticated => false;

    public Guid UserId => Guid.Empty;
}
