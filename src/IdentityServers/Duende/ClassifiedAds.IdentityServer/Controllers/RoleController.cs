using System;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.Application;
using ClassifiedAds.Application.Roles.Commands;
using ClassifiedAds.Application.Roles.Queries;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.Models.RoleModels;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.IdentityServer.Controllers;

public class RoleController : Controller
{
    private readonly Dispatcher _dispatcher;

    public RoleController(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _dispatcher.DispatchAsync(new GetRolesQuery { AsNoTracking = true });
        return View(roles);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        Role role;
        if (id == Guid.Empty)
        {
            role = new Role();
        }
        else
        {
            role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id, AsNoTracking = true });
        }

        var model = role;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Role model)
    {
        Role role;

        if (model.Id == Guid.Empty)
        {
            role = new Role
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
            };

            await _dispatcher.DispatchAsync(new AddUpdateRoleCommand { Role = role });
        }
        else
        {
            role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = model.Id });
            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();
            await _dispatcher.DispatchAsync(new AddUpdateRoleCommand { Role = role });
        }

        return RedirectToAction(nameof(Edit), new { role.Id });
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id, AsNoTracking = true });
        return View(role);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Role model)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = model.Id });
        await _dispatcher.DispatchAsync(new DeleteRoleCommand { Role = role });

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Claims(Guid id)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id, IncludeClaims = true, AsNoTracking = true });

        return View(ClaimsModel.FromEntity(role));
    }

    [HttpPost]
    public async Task<IActionResult> Claims(ClaimModel model)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = model.Role.Id, IncludeClaims = true });

        var claim = new RoleClaim
        {
            Type = model.Type,
            Value = model.Value,
        };

        await _dispatcher.DispatchAsync(new AddClaimCommand { Role = role, Claim = claim });

        return RedirectToAction(nameof(Claims), new { id = role.Id });
    }

    public async Task<IActionResult> DeleteClaim(Guid roleId, Guid claimId)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = roleId, IncludeClaims = true, AsNoTracking = true });
        var claim = role.Claims.FirstOrDefault(x => x.Id == claimId);

        return View(ClaimModel.FromEntity(claim));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteClaim(ClaimModel model)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = model.Role.Id, IncludeClaims = true });

        var claim = role.Claims.FirstOrDefault(x => x.Id == model.Id);

        await _dispatcher.DispatchAsync(new DeleteClaimCommand { Role = role, Claim = claim });

        return RedirectToAction(nameof(Claims), new { id = role.Id });
    }

    public async Task<IActionResult> Users(Guid id)
    {
        var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id, IncludeUsers = true, AsNoTracking = true });

        var users = role.UserRoles.Select(x => x.User).ToList();
        var model = new UsersModel
        {
            Role = role,
            Users = users,
        };

        return View(model);
    }
}