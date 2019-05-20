using System;
using ClassifiedAds.Domain.Entities;
using System.Linq;
using ClassifiedAds.CrossCuttingConcerns.Exceptions;

namespace ClassifiedAds.DomainServices
{
    public class UserService : IUserService
    {
        private IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
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
