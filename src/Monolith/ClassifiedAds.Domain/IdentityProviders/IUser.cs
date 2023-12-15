namespace ClassifiedAds.Domain.IdentityProviders;

public interface IUser
{
    string Id { get; set; }

    string Username { get; set; }

    string Email { get; set; }

    string Password { get; set; }

    string FirstName { get; set; }

    string LastName { get; set; }
}

public class User : IUser
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}
