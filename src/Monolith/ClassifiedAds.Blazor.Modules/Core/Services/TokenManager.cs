using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Core.Services;

public interface ITokenManager
{
    bool AttachTokenAutomatically { get; }

    Task<TokenModel> GetToken();

    Task RefreshToken();
}

public class TokenModel
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public bool TokenExpired
    {
        get
        {
            return ExpiresAt.ToUniversalTime() <= DateTime.UtcNow.AddSeconds(60);
        }
    }
}
