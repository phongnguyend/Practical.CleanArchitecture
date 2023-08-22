using ClassifiedAds.Domain.IdentityProviders;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.IdentityProviders.Auth0;

public class Auth0IdentityProvider : IAuth0IdentityProvider
{
    private readonly Auth0Options _options;
    private HttpClient _httpClient = new HttpClient();

    public Auth0IdentityProvider(Auth0Options options)
    {
        _options = options;
    }

    public async Task<string> GetAccessToken()
    {
        var paramters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret },
            { "audience", _options.Audience }
        };

        var content = new FormUrlEncodedContent(paramters);
        var response = await _httpClient.PostAsync(_options.TokenUrl, content);
        var responseText = await response.Content.ReadAsStringAsync();
        var tokens = JsonSerializer.Deserialize<Dictionary<string, object>>(responseText);
        return tokens["access_token"].ToString();
    }

    public async Task SetAccessToken()
    {
        var accessToken = await GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task CreateUserAsync(IUser user)
    {
        await SetAccessToken();

        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var content = new StringContent(JsonSerializer.Serialize(new Auth0User
        {
            UserId = user.Id,
            Password = user.Password,
            Email = user.Email,
            GivenName = user.FirstName,
            FamilyName = user.LastName,
            Name = $"{user.FirstName} {user.LastName}",
            Connection = "Username-Password-Authentication"
        }, options));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync(_options.Audience + "users", content);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();

        var createdUser = JsonSerializer.Deserialize<Auth0User>(responseText);

        user.Id = createdUser.UserId;
    }

    public async Task DeleteUserAsync(string userId)
    {
        await SetAccessToken();

        var response = await _httpClient.DeleteAsync(_options.Audience + $"users/{userId}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<IUser> GetUserById(string userId)
    {
        await SetAccessToken();

        var response = await _httpClient.GetAsync(_options.Audience + $"users/{userId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        var responseText = await response.Content.ReadAsStringAsync();

        var user = JsonSerializer.Deserialize<Auth0User>(responseText);

        return new User
        {
            Id = user.UserId,
            Username = user.Email,
            Email = user.Email,
        };
    }

    public async Task<IUser> GetUserByUsernameAsync(string username)
    {
        await SetAccessToken();

        var response = await _httpClient.GetAsync(_options.Audience + $"users-by-email?email={username}");
        var responseText = await response.Content.ReadAsStringAsync();

        var users = JsonSerializer.Deserialize<List<Auth0User>>(responseText);

        if (users.Count == 0)
        {
            return null;
        }

        var user = users.First();

        return new User
        {
            Id = user.UserId,
            Username = user.Email,
            Email = user.Email,
        };
    }

    public async Task<IList<IUser>> GetUsersAsync()
    {
        await SetAccessToken();

        var response = await _httpClient.GetAsync(_options.Audience + "users");
        var responseText = await response.Content.ReadAsStringAsync();

        var users = JsonSerializer.Deserialize<List<Auth0User>>(responseText);

        return users.Select(x => (IUser)new User
        {
            Id = x.UserId,
            Username = x.Email,
            Email = x.Email,
        }).ToList();
    }

    public async Task UpdateUserAsync(string userId, IUser user)
    {
        await SetAccessToken();

        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var content = new StringContent(JsonSerializer.Serialize(new Auth0User
        {
            GivenName = user.FirstName,
            FamilyName = user.LastName,
            Name = $"{user.FirstName} {user.LastName}"
        }, options));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PatchAsync(_options.Audience + $"users/{userId}", content);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();

        var updatedUser = JsonSerializer.Deserialize<Auth0User>(responseText);
    }
}
