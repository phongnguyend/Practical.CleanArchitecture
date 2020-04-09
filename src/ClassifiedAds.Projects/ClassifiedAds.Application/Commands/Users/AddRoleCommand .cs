using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;

namespace ClassifiedAds.Application.Commands.Users
{
    public class AddRoleCommand : ICommand
    {
        public User User { get; set; }
        public UserRole Role { get; set; }
    }

    internal class AddRoleCommandHandler : ICommandHandler<AddRoleCommand>
    {
        private readonly IUserService _userService;

        public AddRoleCommandHandler(IUserService productService)
        {
            _userService = productService;
        }

        public void Handle(AddRoleCommand command)
        {
            command.User.UserRoles.Add(command.Role);
            _userService.AddOrUpdate(command.User);
        }
    }
}
