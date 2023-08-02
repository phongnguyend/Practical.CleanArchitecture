using System.Text.Json.Serialization;

namespace ClassifiedAds.Infrastructure.IdentityProviders.Auth0;

public class Auth0User
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; }

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("connection")]
    public string Connection { get; set; }
}
