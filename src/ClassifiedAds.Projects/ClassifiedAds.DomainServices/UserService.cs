using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.DomainServices
{
    public class UserService : IUserService
    {
        private IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
