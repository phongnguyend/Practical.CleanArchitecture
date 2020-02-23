using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Linq;

namespace ClassifiedAds.Domain
{
    public class UserService : IUserService
    {
        private IRepository<User, Guid> _userRepository;

        public UserService(IRepository<User, Guid> userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetUserById(Guid Id)
        {
            ValidationException.Requires(Id != Guid.Empty, "Invalid Id");

            return _userRepository.GetAll().Where(x => x.Id == Id).FirstOrDefault();
        }
    }
}
