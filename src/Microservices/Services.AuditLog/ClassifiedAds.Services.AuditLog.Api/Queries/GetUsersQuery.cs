using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Identity.Contracts.DTOs;
using ClassifiedAds.Services.Identity.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Services.Identity.Contracts.Queries
{
    public class GetUsersQuery : IQuery<List<UserDTO>>
    {
        public bool IncludeClaims { get; set; }
        public bool IncludeUserRoles { get; set; }
        public bool IncludeRoles { get; set; }
        public bool AsNoTracking { get; set; }
    }

    public class GetAuditEntriesQueryHandler : IQueryHandler<GetUsersQuery, List<UserDTO>>
    {
        public List<UserDTO> Handle(GetUsersQuery query)
        {
            var client = new User.UserClient(ChannelFactory.Create("https://localhost:5001"));
            var response = client.GetUsersAsync(new GetUsersRequest()).GetAwaiter().GetResult();

            return response.Users.Select(x => new UserDTO
            {
                Id = Guid.Parse(x.Id),
                UserName = x.UserName,
                Email = x.Email,
            }).ToList();
        }
    }
}
