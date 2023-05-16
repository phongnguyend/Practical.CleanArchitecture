namespace ClassifiedAds.IdentityServer.Models.Accounts;

public class LoginModel
{
    public string ReturnUrl { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool RememberLogin { get; set; }
}
