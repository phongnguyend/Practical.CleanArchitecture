﻿using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Repositories;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Users
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

        public async Task HandleAsync(AddClaimCommand command)
        {
            command.User.Claims.Add(command.Claim);
            await _userRepository.AddOrUpdateAsync(command.User);
            await _userRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
