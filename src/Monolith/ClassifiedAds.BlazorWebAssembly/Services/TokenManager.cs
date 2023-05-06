using ClassifiedAds.Blazor.Modules.Core.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;

namespace ClassifiedAds.BlazorWebAssembly.Services;

public class TokenManager : ITokenManager
{
    private readonly IAccessTokenProvider _tokenProvider;

    public TokenManager(IAccessTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public bool AttachTokenAutomatically => true;

    public async Task<TokenModel> GetToken()
    {
        var accessTokenResult = await _tokenProvider.RequestAccessToken();

        if (accessTokenResult.TryGetToken(out var token))
        {
            return new TokenModel { AccessToken = token.Value, ExpiresAt = token.Expires };
        }

        return null;
    }

    public async Task RefreshToken()
    {
    }
}
