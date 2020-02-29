using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public enum ClientType
    {
        Empty = 0,
        WebHybrid = 1,
        Spa = 2,
        Native = 3,
        Machine = 4,
        Device = 5,
    }

    public class ClientModel : IdentityServer4.Models.Client
    {
        public int Id { get; set; }
        public ClientType ClientType { get; set; }
        public string AllowedGrantTypesItems { get; set; }
        public string RedirectUrisItems { get; set; }
        public string PostLogoutRedirectUrisItems { get; set; }
        public string AllowedScopesItems { get; set; }
        public string IdentityProviderRestrictionsItems { get; set; }
        public string AllowedCorsOriginsItems { get; set; }

        public string OriginalClientId { get; set; }

        public static ClientModel FromEntity(IdentityServer4.EntityFramework.Entities.Client client)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<IdentityServer4.Models.Client, ClientModel>());
            var mapper = config.CreateMapper();
            var model = mapper.Map<ClientModel>(client.ToModel());
            model.Id = client.Id;
            return model;
        }

        public void SetDefaultValues()
        {
            switch (ClientType)
            {
                case ClientType.Empty:
                    break;
                case ClientType.WebHybrid:
                    AllowedGrantTypes = AllowedGrantTypes.ToList().Concat(GrantTypes.Hybrid).ToList();
                    break;
                case ClientType.Spa:
                    AllowedGrantTypes = AllowedGrantTypes.ToList().Concat(GrantTypes.Code).ToList();
                    RequirePkce = true;
                    RequireClientSecret = false;
                    break;
                case ClientType.Native:
                    AllowedGrantTypes = AllowedGrantTypes.ToList().Concat(GrantTypes.Hybrid).ToList();
                    break;
                case ClientType.Machine:
                    AllowedGrantTypes = AllowedGrantTypes.ToList().Concat(GrantTypes.ResourceOwnerPasswordAndClientCredentials).ToList();
                    break;
                case ClientType.Device:
                    AllowedGrantTypes = AllowedGrantTypes.ToList().Concat(GrantTypes.DeviceFlow).ToList();
                    RequireClientSecret = false;
                    AllowOfflineAccess = true;
                    break;
            }
        }

        public void ConvertItemsToList()
        {
            AllowedGrantTypes = ConvertItems(AllowedGrantTypesItems);
            RedirectUris = ConvertItems(RedirectUrisItems);
            PostLogoutRedirectUris = ConvertItems(PostLogoutRedirectUrisItems);
            AllowedScopes = ConvertItems(AllowedScopesItems);
            IdentityProviderRestrictions = ConvertItems(IdentityProviderRestrictionsItems);
            AllowedCorsOrigins = ConvertItems(AllowedCorsOriginsItems);
        }

        private List<string> ConvertItems(string items)
        {
            return string.IsNullOrWhiteSpace(items)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(items);
        }

        public void UpdateEntity(IdentityServer4.EntityFramework.Entities.Client entity)
        {
            entity.Enabled = Enabled;
            entity.ClientId = ClientId;
            entity.ProtocolType = ProtocolType;
            entity.RequireClientSecret = RequireClientSecret;
            entity.ClientName = ClientName;
            entity.Description = Description;
            entity.ClientUri = ClientUri;
            entity.LogoUri = LogoUri;
            entity.RequireConsent = RequireConsent;
            entity.AllowRememberConsent = AllowRememberConsent;
            entity.AlwaysIncludeUserClaimsInIdToken = AlwaysIncludeUserClaimsInIdToken;
            entity.AllowedGrantTypes = AllowedGrantTypes.Select(x => new ClientGrantType
            {
                GrantType = x,
            }).ToList();
            entity.RequirePkce = RequirePkce;
            entity.AllowPlainTextPkce = AllowPlainTextPkce;
            entity.AllowAccessTokensViaBrowser = AllowAccessTokensViaBrowser;
            entity.RedirectUris = RedirectUris.Select(x => new ClientRedirectUri
            {
                RedirectUri = x,
            }).ToList();
            entity.PostLogoutRedirectUris = PostLogoutRedirectUris.Select(x => new ClientPostLogoutRedirectUri
            {
                PostLogoutRedirectUri = x,
            }).ToList();
            entity.FrontChannelLogoutUri = FrontChannelLogoutUri;
            entity.FrontChannelLogoutSessionRequired = FrontChannelLogoutSessionRequired;
            entity.BackChannelLogoutUri = BackChannelLogoutUri;
            entity.BackChannelLogoutSessionRequired = BackChannelLogoutSessionRequired;
            entity.AllowOfflineAccess = AllowOfflineAccess;
            entity.AllowedScopes = AllowedScopes.Select(x => new ClientScope
            {
                Scope = x,
            }).ToList();
            entity.IdentityTokenLifetime = IdentityTokenLifetime;
            entity.AccessTokenLifetime = AccessTokenLifetime;
            entity.AuthorizationCodeLifetime = AuthorizationCodeLifetime;
            entity.ConsentLifetime = ConsentLifetime;
            entity.AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime;
            entity.SlidingRefreshTokenLifetime = SlidingRefreshTokenLifetime;
            entity.RefreshTokenUsage = (int)RefreshTokenUsage;
            entity.UpdateAccessTokenClaimsOnRefresh = UpdateAccessTokenClaimsOnRefresh;
            entity.RefreshTokenExpiration = (int)RefreshTokenExpiration;
            entity.AccessTokenType = (int)AccessTokenType;
            entity.EnableLocalLogin = EnableLocalLogin;
            entity.IdentityProviderRestrictions = IdentityProviderRestrictions.Select(x => new ClientIdPRestriction
            {
                Provider = x,
            }).ToList();
            entity.IncludeJwtId = IncludeJwtId;
            entity.AlwaysSendClientClaims = AlwaysSendClientClaims;
            entity.ClientClaimsPrefix = ClientClaimsPrefix;
            entity.PairWiseSubjectSalt = PairWiseSubjectSalt;
            entity.AllowedCorsOrigins = AllowedCorsOrigins.Select(x => new ClientCorsOrigin
            {
                Origin = x,
            }).ToList();
            entity.UserSsoLifetime = UserSsoLifetime;
            entity.UserCodeType = UserCodeType;
            entity.DeviceCodeLifetime = DeviceCodeLifetime;
        }
    }
}
