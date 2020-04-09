using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Users
{
    public class DeleteClaimCommand : ICommand
    {
        public User User { get; set; }
        public UserClaim Claim { get; set; }
    }

    internal class DeleteClaimCommandHandler : ICommandHandler<DeleteClaimCommand>
    {
        private readonly IUserService _userService;

        public DeleteClaimCommandHandler(IUserService productService)
        {
            _userService = productService;
        }

        public void Handle(DeleteClaimCommand command)
        {
            command.User.Claims.Remove(command.Claim);
            _userService.AddOrUpdate(command.User);
        }
    }
}
