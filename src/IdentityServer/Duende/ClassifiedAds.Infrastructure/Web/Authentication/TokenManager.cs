using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Authentication
{
    public class TokenManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OpenIdConnectOptions _options;

        public TokenManager(IHttpClientFactory httpClientFactory, OpenIdConnectOptions options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public async Task<TokenModel> RefreshToken(string refreshToken)
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
                RefreshToken = refreshToken,
            });

            if (response.IsError)
            {
                if (response.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return null;
                }

                throw new Exception(response.Raw);
            }

            return new TokenModel
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn),
            };
        }
    }
}
