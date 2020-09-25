using System;

namespace ClassifiedAds.Modules.Identity.Contracts.Services
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        Guid UserId { get; }
    }
}
