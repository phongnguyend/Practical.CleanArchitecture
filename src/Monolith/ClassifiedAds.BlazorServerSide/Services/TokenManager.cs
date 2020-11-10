using ClassifiedAds.Blazor.Modules.Core.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.BlazorServerSide.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenIdConnectOptions _options;
        private readonly TokenProvider _tokenProvider;

        public TokenManager(IHttpClientFactory httpClientFactory, OpenIdConnectOptions options, TokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
            _tokenProvider = tokenProvider;
        }

        public bool AttachTokenAutomatically => false;

        public Task<TokenModel> GetToken()
        {
            return Task.FromResult(new TokenModel
            {
                AccessToken = _tokenProvider.AccessToken,
                RefreshToken = _tokenProvider.RefreshToken,
                ExpiresAt = _tokenProvider.ExpiresAt,
            });
        }

        public async Task RefreshToken()
        {
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
        }
    }
}
