using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.IdentityServer.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}