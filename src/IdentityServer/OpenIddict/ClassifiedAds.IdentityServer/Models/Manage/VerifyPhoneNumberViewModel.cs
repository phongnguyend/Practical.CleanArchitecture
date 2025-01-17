using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.IdentityServer.Models.Manage;

public class VerifyPhoneNumberViewModel
{
    [Required]
    public string Code { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
}
