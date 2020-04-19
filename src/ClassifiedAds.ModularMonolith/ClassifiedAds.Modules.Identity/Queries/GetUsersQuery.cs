using ClassifiedAds.Modules.Identity.Contracts.DTOs;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Application.Users.Queries
{
    public class GetUsersQuery : IQuery<List<User>>
    {
        public bool IncludeClaims { get; set; }
        public bool IncludeUserRoles { get; set; }
        public bool IncludeRoles { get; set; }
        public bool AsNoTracking { get; set; }
    }

    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, List<User>>, IQueryHandler<Modules.Identity.Contracts.Queries.GetUsersQuery, List<UserDTO>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> Handle(GetUsersQuery query)
        {
            var db = _userRepository.Get(new UserQueryOptions
            {
                IncludeClaims = query.IncludeClaims,
                IncludeUserRoles = query.IncludeUserRoles,
                IncludeRoles = query.IncludeRoles,
                AsNoTracking = query.AsNoTracking,
            });

            return db.ToList();
        }

        public List<UserDTO> Handle(Modules.Identity.Contracts.Queries.GetUsersQuery query)
        {
            var db = _userRepository.Get(new UserQueryOptions
            {
                IncludeClaims = query.IncludeClaims,
                IncludeUserRoles = query.IncludeUserRoles,
                IncludeRoles = query.IncludeRoles,
                AsNoTracking = query.AsNoTracking,
            }).Select(x => new UserDTO
            {
                Id = x.Id,
                UserName = x.UserName,
            });

            return db.ToList();
        }
    }
}
