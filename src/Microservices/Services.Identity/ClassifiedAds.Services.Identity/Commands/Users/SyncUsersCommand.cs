using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.IdentityProviders;
using ClassifiedAds.Services.Identity.IdentityProviders.Auth0;
using ClassifiedAds.Services.Identity.IdentityProviders.Azure;
using ClassifiedAds.Services.Identity.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Commands.Users;

public class SyncUsersCommand : ICommand
{
    public int SyncedUsersCount { get; set; }
}

public class SyncUsersCommandHandler : ICommandHandler<SyncUsersCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IServiceProvider _serviceProvider;

    public SyncUsersCommandHandler(IUserRepository userRepository,
        IServiceProvider serviceProvider)
    {
        _userRepository = userRepository;
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(SyncUsersCommand command, CancellationToken cancellationToken = default)
    {
        await SyncToAuth0(command);

        await SyncToAzureAdB2C(command);
    }

    private async Task SyncToAzureAdB2C(SyncUsersCommand command)
    {
        var manager = (AzureActiveDirectoryB2CManager)_serviceProvider.GetService(typeof(AzureActiveDirectoryB2CManager));

        if (manager is null)
        {
            return;
        }

        var users = _userRepository.GetAll()
             .Where(x => x.AzureAdB2CUserId == null)
             .Take(50)
             .ToList();

        foreach (var user in users)
        {
            var existingUser = await manager.GetUserByUsernameAsync(user.UserName);

            if (existingUser != null)
            {
                user.AzureAdB2CUserId = existingUser.Id;
            }
            else
            {
                var newUser = new User
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Password = Guid.NewGuid().ToString(),
                    FirstName = "FirstName",
                    LastName = "LastName"
                };

                await manager.CreateUserAsync(newUser);

                user.AzureAdB2CUserId = newUser.Id;
            }

            await _userRepository.UnitOfWork.SaveChangesAsync();
        }

        command.SyncedUsersCount += users.Count;
    }

    private async Task SyncToAuth0(SyncUsersCommand command)
    {
        var manager = (Auth0Manager)_serviceProvider.GetService(typeof(Auth0Manager));

        if (manager is null)
        {
            return;
        }

        var users = _userRepository.GetAll()
            .Where(x => x.Auth0UserId == null)
            .Take(50)
            .ToList();

        foreach (var user in users)
        {
            var existingUser = await manager.GetUserByUsernameAsync(user.UserName);

            if (existingUser != null)
            {
                user.Auth0UserId = existingUser.Id;
            }
            else
            {
                var newUser = new User
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Password = Guid.NewGuid().ToString()
                };

                await manager.CreateUserAsync(newUser);

                user.Auth0UserId = newUser.Id;
            }

            await _userRepository.UnitOfWork.SaveChangesAsync();
        }

        command.SyncedUsersCount += users.Count;
    }
}
