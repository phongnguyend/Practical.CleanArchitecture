using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.BlazorServerSide.ConfigurationOptions;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.BlazorServerSide.Services;

public class TokenManager : ITokenManager
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OpenIdConnectOptions _options;
    private readonly TokenProvider _tokenProvider;
    private readonly ProtectedLocalStorage _protectedLocalStorage;

    public TokenManager(AppSettings appSettings, IHttpClientFactory httpClientFactory, TokenProvider tokenProvider, ProtectedLocalStorage protectedLocalStorage)
    {
        _httpClientFactory = httpClientFactory;
        _options = new OpenIdConnectOptions
        {
            Authority = appSettings.OpenIdConnect.Authority,
            ClientId = appSettings.OpenIdConnect.ClientId,
            ClientSecret = appSettings.OpenIdConnect.ClientSecret,
            RequireHttpsMetadata = appSettings.OpenIdConnect.RequireHttpsMetadata,
        };
        _tokenProvider = tokenProvider;
        _protectedLocalStorage = protectedLocalStorage;
    }

    public bool AttachTokenAutomatically => false;

    public async Task<TokenModel> GetToken()
    {
        int count = 0;
        while (!_tokenProvider.IsReady)
        {
            await Task.Delay(1000);
            count++;

            if (count > 60)
            {
                break;
            }
        }

        return new TokenModel
        {
            AccessToken = _tokenProvider.AccessToken,
            RefreshToken = _tokenProvider.RefreshToken,
            ExpiresAt = _tokenProvider.ExpiresAt,
        };
    }

    public async Task RefreshToken()
    {
        if (string.IsNullOrEmpty(_tokenProvider.RefreshToken))
        {
            return;
        }

        var httpClient = _httpClientFactory.CreateClient();
        var metaDataResponse = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _options.Authority,
            Policy = { RequireHttps = _options.RequireHttpsMetadata },
        });

        var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = metaDataResponse.TokenEndpoint,
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            RefreshToken = _tokenProvider.RefreshToken,
        });

        if (response.IsError)
        {
            if (response.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return;
            }

            throw new Exception(response.Raw);
        }

        _tokenProvider.AccessToken = response.AccessToken;
        _tokenProvider.RefreshToken = response.RefreshToken;
        _tokenProvider.ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);

        await _protectedLocalStorage.SetAsync("tokens", _tokenProvider);
    }
}
