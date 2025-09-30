using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.IdentityServer.Models.Manage;

public class AddPhoneNumberViewModel
{
    [Required]
    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }
}
