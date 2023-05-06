using ClassifiedAds.WebMVC.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.HttpMessageHandlers;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsSnapshot<AppSettings> _appSettings;

    public BearerTokenHandler(IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IOptionsSnapshot<AppSettings> appSettings)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        var identityToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
        var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
        var expiresAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}
