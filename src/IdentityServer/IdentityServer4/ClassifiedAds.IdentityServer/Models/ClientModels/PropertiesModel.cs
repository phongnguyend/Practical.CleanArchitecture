using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public class PropertiesModel : PropertyModel
    {
        public List<PropertyModel> Properties { get; set; }

        public static PropertiesModel FromEntity(Client client)
        {
            var clientModel = ClientModel.FromEntity(client);
            return new PropertiesModel
            {
                Client = clientModel,
                Properties = client.Properties?.Select(x => new PropertyModel
                {
                    Id = x.Id,
                    Key = x.Key,
                    Value = x.Value,
                    Client = clientModel,
                })?.ToList(),
            };
        }
    }
}
