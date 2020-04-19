using System;
using System.Linq;
using ClassifiedAds.Application;
using ClassifiedAds.Application.Roles.Commands;
using ClassifiedAds.Application.Roles.Queries;
using ClassifiedAds.IdentityServer.Models.RoleModels;
using ClassifiedAds.Modules.Identity.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class RoleController : Controller
    {
        private readonly Dispatcher _dispatcher;

        public RoleController(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IActionResult Index()
        {
            var roles = _dispatcher.Dispatch(new GetRolesQuery { AsNoTracking = true });
            return View(roles);
        }

        public IActionResult Edit(Guid id)
        {
            Role role;
            if (id == Guid.Empty)
            {
                role = new Role();
            }
            else
            {
                role = _dispatcher.Dispatch(new GetRoleQuery { Id = id, AsNoTracking = true });
            }

            var model = role;

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Role model)
        {
            Role role;

            if (model.Id == Guid.Empty)
            {
                role = new Role
                {
                    Name = model.Name,
                    NormalizedName = model.Name.ToUpper(),
                };

                _dispatcher.Dispatch(new AddUpdateRoleCommand { Role = role });
            }
            else
            {
                role = _dispatcher.Dispatch(new GetRoleQuery { Id = model.Id });
                role.Name = model.Name;
                role.NormalizedName = model.Name.ToUpper();
                _dispatcher.Dispatch(new AddUpdateRoleCommand { Role = role });
            }

            return RedirectToAction(nameof(Edit), new { role.Id });
        }

        public IActionResult Delete(Guid id)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id, AsNoTracking = true });
            return View(role);
        }

        [HttpPost]
        public IActionResult Delete(Role model)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = model.Id });
            _dispatcher.Dispatch(new DeleteRoleCommand { Role = role });

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Claims(Guid id)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id, IncludeClaims = true, AsNoTracking = true });

            return View(ClaimsModel.FromEntity(role));
        }

        [HttpPost]
        public IActionResult Claims(ClaimModel model)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = model.Role.Id, IncludeClaims = true });

            var claim = new RoleClaim
            {
                Type = model.Type,
                Value = model.Value,
            };

            _dispatcher.Dispatch(new AddClaimCommand { Role = role, Claim = claim });

            return RedirectToAction(nameof(Claims), new { id = role.Id });
        }

        public IActionResult DeleteClaim(Guid roleId, Guid claimId)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = roleId, IncludeClaims = true, AsNoTracking = true });
            var claim = role.Claims.FirstOrDefault(x => x.Id == claimId);

            return View(ClaimModel.FromEntity(claim));
        }

        [HttpPost]
        public IActionResult DeleteClaim(ClaimModel model)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = model.Role.Id, IncludeClaims = true });

            var claim = role.Claims.FirstOrDefault(x => x.Id == model.Id);

            _dispatcher.Dispatch(new DeleteClaimCommand { Role = role, Claim = claim });

            return RedirectToAction(nameof(Claims), new { id = role.Id });
        }

        public IActionResult Users(Guid id)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id, IncludeUsers = true, AsNoTracking = true });

            var users = role.UserRoles.Select(x => x.User).ToList();
            var model = new UsersModel
            {
                Role = role,
                Users = users,
            };

            return View(model);
        }
    }
}