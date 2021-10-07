using ClassifiedAds.Modules.Identity.Contracts.Services;
using System;

namespace ClassifiedAds.BackgroundServer.Identity
{
    public class CurrentUser : ICurrentUser
    {
        public bool IsAuthenticated => false;

        public Guid UserId => Guid.Empty;
    }
}
