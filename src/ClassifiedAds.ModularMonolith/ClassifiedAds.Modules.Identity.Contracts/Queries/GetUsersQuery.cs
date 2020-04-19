using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Identity.Contracts.Queries
{
    public class GetUsersQuery : IQuery<List<UserDTO>>
    {
        public bool IncludeClaims { get; set; }
        public bool IncludeUserRoles { get; set; }
        public bool IncludeRoles { get; set; }
        public bool AsNoTracking { get; set; }
    }
}
