using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Domain.Services
{
    public class UserService : CrudService<User>, IUserService
    {
        public UserService(IRepository<User, Guid> userRepository)
            : base(userRepository)
        {
        }
    }
}
