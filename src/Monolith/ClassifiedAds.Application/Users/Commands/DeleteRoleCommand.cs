﻿using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.Users.Commands
{
    public class DeleteRoleCommand : ICommand
    {
        public User User { get; set; }
        public UserRole Role { get; set; }
    }

    internal class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteRoleCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(DeleteRoleCommand command)
        {
            command.User.UserRoles.Remove(command.Role);
            await _userRepository.AddOrUpdateAsync(command.User);
            await _userRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
