using IdentityServer4.EntityFramework.Entities;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public class ClaimModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public ClientModel Client { get; set; }

        public static ClaimModel FromEntity(ClientClaim claim)
        {
            return new ClaimModel
            {
                Id = claim.Id,
                Type = claim.Type,
                Value = claim.Value,
                Client = ClientModel.FromEntity(claim.Client),
            };
        }
    }
}
