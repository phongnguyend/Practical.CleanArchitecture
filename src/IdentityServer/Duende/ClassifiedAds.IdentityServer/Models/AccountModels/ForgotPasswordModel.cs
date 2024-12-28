using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.IdentityServer.Models.AccountModels;

public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}