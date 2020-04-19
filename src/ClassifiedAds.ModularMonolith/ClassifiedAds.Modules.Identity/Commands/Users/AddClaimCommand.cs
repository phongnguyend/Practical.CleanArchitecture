using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;

namespace ClassifiedAds.Application.Users.Commands
{
    public class AddClaimCommand : ICommand
    {
        public User User { get; set; }
        public UserClaim Claim { get; set; }
    }

    public class AddClaimCommandHandler : ICommandHandler<AddClaimCommand>
    {
        private readonly IUserRepository _userRepository;

        public AddClaimCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(AddClaimCommand command)
        {
            command.User.Claims.Add(command.Claim);
            _userRepository.AddOrUpdate(command.User);
            _userRepository.UnitOfWork.SaveChanges();
        }
    }
}
