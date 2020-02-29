using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.IdentityServer.Models.UserModels
{
    public class ClaimModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public User User { get; set; }

        public static ClaimModel FromEntity(UserClaim claim)
        {
            return new ClaimModel
            {
                Id = claim.Id,
                Type = claim.Type,
                Value = claim.Value,
                User = claim.User,
            };
        }
    }
}
