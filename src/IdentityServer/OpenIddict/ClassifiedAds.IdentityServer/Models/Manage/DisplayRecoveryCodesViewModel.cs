using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.IdentityServer.Models.Manage;

public class DisplayRecoveryCodesViewModel
{
    [Required]
    public IEnumerable<string> Codes { get; set; }

}
