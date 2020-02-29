using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.Models.RoleModels;
using ClassifiedAds.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class RoleController : Controller
    {
        private readonly AdsDbContext _dbContext;

        public RoleController(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var roles = _dbContext.Set<Role>().AsNoTracking().ToList();
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
                role = _dbContext.Set<Role>().FirstOrDefault(x => x.Id == id);
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

                _dbContext.Set<Role>().Add(role);
            }
            else
            {
                role = _dbContext.Set<Role>().FirstOrDefault(x => x.Id == model.Id);
                role.Name = model.Name;
                role.NormalizedName = model.Name.ToUpper();
            }

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), new { role.Id });
        }

        public IActionResult Delete(Guid id)
        {
            var role = _dbContext.Set<Role>().FirstOrDefault(x => x.Id == id);
            return View(role);
        }

        [HttpPost]
        public IActionResult Delete(Role model)
        {
            var role = _dbContext.Set<Role>().FirstOrDefault(x => x.Id == model.Id);
            _dbContext.Set<Role>().Remove(role);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Claims(Guid id)
        {
            var role = _dbContext.Set<Role>()
                .Include(x => x.Claims)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

            return View(ClaimsModel.FromEntity(role));
        }

        [HttpPost]
        public IActionResult Claims(ClaimModel model)
        {
            var role = _dbContext.Set<Role>()
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == model.Role.Id);

            role.Claims.Add(new RoleClaim
            {
                Type = model.Type,
                Value = model.Value,
            });

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Claims), new { id = role.Id });
        }

        public IActionResult DeleteClaim(Guid id)
        {
            var claim = _dbContext.Set<RoleClaim>()
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == id);

            return View(ClaimModel.FromEntity(claim));
        }

        [HttpPost]
        public IActionResult DeleteClaim(ClaimModel model)
        {
            var role = _dbContext.Set<Role>()
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == model.Role.Id);

            var claim = role.Claims.FirstOrDefault(x => x.Id == model.Id);

            role.Claims.Remove(claim);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Claims), new { id = role.Id });
        }

        public IActionResult Users(Guid id)
        {
            var role = _dbContext.Set<Role>()
                .Include("UserRoles.User")
                .FirstOrDefault(x => x.Id == id);

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