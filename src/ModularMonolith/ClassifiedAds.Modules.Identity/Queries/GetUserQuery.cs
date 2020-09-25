using ClassifiedAds.Application;
using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System;
using System.Linq;

namespace ClassifiedAds.Modules.Identity.Queries.Roles
{
    public class GetUserQuery : IQuery<User>
    {
        public Guid Id { get; set; }
        public bool IncludeClaims { get; set; }
        public bool IncludeUserRoles { get; set; }
        public bool IncludeRoles { get; set; }
        public bool AsNoTracking { get; set; }
    }

    public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
    {
        private readonly IUserRepository _userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Handle(GetUserQuery query)
        {
            var db = _userRepository.Get(new UserQueryOptions
            {
                IncludeClaims = query.IncludeClaims,
                IncludeUserRoles = query.IncludeUserRoles,
                IncludeRoles = query.IncludeRoles,
                AsNoTracking = query.AsNoTracking,
            });

            return db.FirstOrDefault(x => x.Id == query.Id);
        }
    }
}
