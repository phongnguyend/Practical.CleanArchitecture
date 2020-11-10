using System;

namespace ClassifiedAds.Blazor.Modules.Core.Services
{
    public class TokenProvider
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public bool TokenExpired
        {
            get
            {
                return ExpiresAt.AddSeconds(-60).ToUniversalTime() <= DateTime.UtcNow;
            }
        }
    }
}
