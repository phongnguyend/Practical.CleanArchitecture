using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Application.Users.Services;

public class UserService : CrudService<User>, IUserService
{
    public UserService(IRepository<User, Guid> userRepository, Dispatcher dispatcher)
        : base(userRepository, dispatcher)
    {
    }
}
