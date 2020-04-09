using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Users
{
    public class AddClaimCommand : ICommand
    {
        public User User { get; set; }
        public UserClaim Claim { get; set; }
    }

    internal class AddClaimCommandHandler : ICommandHandler<AddClaimCommand>
    {
        private readonly IUserService _userService;

        public AddClaimCommandHandler(IUserService productService)
        {
            _userService = productService;
        }

        public void Handle(AddClaimCommand command)
        {
            command.User.Claims.Add(command.Claim);
            _userService.AddOrUpdate(command.User);
        }
    }
}
