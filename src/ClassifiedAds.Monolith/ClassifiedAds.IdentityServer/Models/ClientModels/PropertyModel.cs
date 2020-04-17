using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public class PropertyModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public ClientModel Client { get; set; }

        public static PropertyModel FromEntity(ClientProperty prop)
        {
            return new PropertyModel
            {
                Id = prop.Id,
                Key = prop.Key,
                Value = prop.Value,
                Client = ClientModel.FromEntity(prop.Client),
            };
        }
    }
}
