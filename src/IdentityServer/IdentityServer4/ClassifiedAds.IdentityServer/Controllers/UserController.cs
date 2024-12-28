using ClassifiedAds.Application;
using ClassifiedAds.Application.Roles.Queries;
using ClassifiedAds.Application.Users.Commands;
using ClassifiedAds.Application.Users.Queries;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Controllers;

public class UserController : Controller
{
    private readonly Dispatcher _dispatcher;
    private readonly UserManager<User> _userManager;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserController(Dispatcher dispatcher,
        UserManager<User> userManager,
        ILogger<UserController> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _dispatcher = dispatcher;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        logger.LogInformation("UserController");
    }

    public async Task<IActionResult> Index()
    {
        var users = await _dispatcher.DispatchAsync(new GetUsersQuery { AsNoTracking = true });
        return View(users);
    }

    public async Task<IActionResult> Profile(Guid id)
    {
        var user = id != Guid.Empty
            ? await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, AsNoTracking = true })
            : new User();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(User model)
    {
        User user;
        if (model.Id != Guid.Empty)
        {
            user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.Id });
        }
        else
        {
            user = new User();
        }

        user.UserName = model.UserName;
        user.NormalizedUserName = model.UserName.ToUpper();
        user.Email = model.Email;
        user.NormalizedEmail = model.Email.ToUpper();
        user.EmailConfirmed = model.EmailConfirmed;
        user.PhoneNumber = model.PhoneNumber;
        user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
        user.TwoFactorEnabled = model.TwoFactorEnabled;
        user.LockoutEnabled = model.LockoutEnabled;
        user.LockoutEnd = model.LockoutEnd;
        user.AccessFailedCount = model.AccessFailedCount;

        _ = model.Id != Guid.Empty
             ? await _userManager.UpdateAsync(user)
             : await _userManager.CreateAsync(user);

        return RedirectToAction(nameof(Profile), new { user.Id });
    }

    public async Task<IActionResult> ChangePassword(Guid id)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, AsNoTracking = true });
        return View(ChangePasswordModel.FromEntity(user));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, AsNoTracking = true });
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(User model)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.Id });
        await _dispatcher.DispatchAsync(new DeleteUserCommand { User = user });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.Id });
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var rs = await _userManager.ResetPasswordAsync(user, token, model.ConfirmPassword);

        if (rs.Succeeded)
        {
            return RedirectToAction(nameof(Profile), new { model.Id });
        }

        foreach (var error in rs.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(ChangePasswordModel.FromEntity(user));
    }

    public async Task<IActionResult> Claims(Guid id)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, IncludeClaims = true, AsNoTracking = true });
        return View(ClaimsModel.FromEntity(user));
    }

    [HttpPost]
    public async Task<IActionResult> Claims(ClaimModel model)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.User.Id, IncludeClaims = true });
        await _dispatcher.DispatchAsync(new AddClaimCommand
        {
            User = user,
            Claim = new UserClaim
            {
                Type = model.Type,
                Value = model.Value,
            },
        });

        return RedirectToAction(nameof(Claims), new { id = user.Id });
    }

    public async Task<IActionResult> DeleteClaim(Guid userId, Guid claimId)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = userId, IncludeClaims = true, AsNoTracking = true });
        var claim = user.Claims.FirstOrDefault(x => x.Id == claimId);

        return View(ClaimModel.FromEntity(claim));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteClaim(ClaimModel model)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.User.Id, IncludeClaims = true });

        var claim = user.Claims.FirstOrDefault(x => x.Id == model.Id);

        await _dispatcher.DispatchAsync(new DeleteClaimCommand
        {
            User = user,
            Claim = claim,
        });

        return RedirectToAction(nameof(Claims), new { id = user.Id });
    }

    public async Task<IActionResult> Roles(Guid id)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, IncludeRoles = true, AsNoTracking = true });

        var roles = await _dispatcher.DispatchAsync(new GetRolesQuery { AsNoTracking = true });

        var model = new RolesModel
        {
            User = user,
            UserRoles = user.UserRoles.Select(x => new RoleModel { Role = x.Role, RoleId = x.RoleId }).ToList(),
            Roles = roles.Where(x => !user.UserRoles.Any(y => y.RoleId == x.Id)).ToList(),
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Roles(RolesModel model)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.User.Id, IncludeUserRoles = true });

        await _dispatcher.DispatchAsync(new AddRoleCommand
        {
            User = user,
            Role = new UserRole
            {
                RoleId = model.Role.RoleId,
            },
        });

        return RedirectToAction(nameof(Roles), new { model.User.Id });
    }

    public async Task<IActionResult> DeleteRole(Guid id, Guid roleId)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, IncludeRoles = true, AsNoTracking = true });
        var role = user.UserRoles.FirstOrDefault(x => x.RoleId == roleId);
        var model = new RoleModel { User = user, Role = role.Role };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole(RoleModel model)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = model.User.Id, IncludeUserRoles = true });

        var role = user.UserRoles.FirstOrDefault(x => x.RoleId == model.Role.Id);

        await _dispatcher.DispatchAsync(new DeleteRoleCommand
        {
            User = user,
            Role = role,
        });

        return RedirectToAction(nameof(Roles), new { model.User.Id });
    }
}