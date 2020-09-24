using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public class ClaimsModel : ClaimModel
    {
        public List<ClaimModel> Claims { get; set; }

        public static ClaimsModel FromEntity(Client client)
        {
            var clientModel = ClientModel.FromEntity(client);

            return new ClaimsModel
            {
                Client = clientModel,
                Claims = client.Claims?.Select(x => FromEntity(x))?.ToList(),
            };
        }
    }
}
