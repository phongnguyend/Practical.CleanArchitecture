using Azure.Identity;
using ClassifiedAds.Domain.IdentityProviders;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.IdentityProviders.Azure;

public class AzureActiveDirectoryB2CIdentityProvider : IAzureActiveDirectoryB2CIdentityProvider
{
    private readonly AzureAdB2COptions _options;
    private readonly GraphServiceClient _graphServiceClient;

    public AzureActiveDirectoryB2CIdentityProvider(AzureAdB2COptions options)
    {
        _options = options;

        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var clientSecretCredential = new ClientSecretCredential(_options.TenantId, _options.AppId, _options.ClientSecret);
        _graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);
    }

    public async Task CreateUserAsync(IUser user)
    {
        try
        {
            var createdUser = await _graphServiceClient.Users.PostAsync(new Microsoft.Graph.Models.User
            {
                PasswordProfile = new Microsoft.Graph.Models.PasswordProfile { Password = user.Password },
                GivenName = user.FirstName,
                Surname = user.LastName,
                DisplayName = $"{user.FirstName} {user.LastName}",
                AccountEnabled = true,
                Mail = user.Email,
                Identities = new List<Microsoft.Graph.Models.ObjectIdentity>
                {
                    new Microsoft.Graph.Models.ObjectIdentity
                    {
                        SignInType = "emailAddress",
                        Issuer = _options.TenantDomain,
                        IssuerAssignedId = user.Username,
                    }
                }
            });

            user.Id = createdUser.Id;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        await _graphServiceClient.Users[userId].DeleteAsync();
    }

    public async Task<IUser> GetUserById(string userId)
    {
        try
        {
            var user = await _graphServiceClient.Users[userId].GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[]
                {
                    "Id",
                    "Mail",
                    "GivenName",
                    "Surname",
                    "Identities"
                };
            });

            return new User
            {
                Id = user.Id,
                Username = user.Mail,
                FirstName = user.GivenName,
                LastName = user.Surname
            };
        }
        catch (ODataError ex)
        {
            if (ex.ResponseStatusCode == 404)
            {
                return null;
            }

            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IUser> GetUserByUsernameAsync(string username)
    {
        try
        {
            var result = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[]
                {
                    "Id",
                    "Mail",
                    "GivenName",
                    "Surname",
                    "Identities"
                };
                requestConfiguration.QueryParameters.Filter = $"identities/any(c:c/issuerAssignedId eq '{username}' and c/issuer eq '{_options.TenantId}')";
            });

            var user = result.Value.FirstOrDefault();

            return user is null ? null : new User
            {
                Id = user.Id,
                Username = username,
                FirstName = user.GivenName,
                LastName = user.Surname
            };
        }
        catch (ODataError ex)
        {
            if (ex.ResponseStatusCode == 404)
            {
                return null;
            }

            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<IList<IUser>> GetUsersAsync()
    {
        var result = await _graphServiceClient.Users.GetAsync();

        return result.Value.Select(x => (IUser)new User
        {
            Id = x.Id,
            Username = x.Mail,
            FirstName = x.GivenName,
            LastName = x.Surname
        }).ToList();
    }

    public async Task UpdateUserAsync(string userId, IUser user)
    {
        var returnedUser = await _graphServiceClient.Users[userId].PatchAsync(new Microsoft.Graph.Models.User
        {
            GivenName = user.FirstName,
            Surname = user.LastName,
            DisplayName = $"{user.FirstName} {user.LastName}"
        });
    }
}
