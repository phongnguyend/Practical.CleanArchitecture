using System;

namespace ClassifiedAds.Domain.Identity
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        Guid UserId { get; }
    }
}
