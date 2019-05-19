using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Domain.Entities
{
    public class User : IdentityUser, IHasKey<string>, ITrackable
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
