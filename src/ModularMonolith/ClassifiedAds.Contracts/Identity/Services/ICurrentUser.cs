using System;

namespace ClassifiedAds.Contracts.Identity.Services;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid UserId { get; }
}
