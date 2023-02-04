using System;

namespace ClassifiedAds.Infrastructure.Web.Authentication
{
    public class TokenModel
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
