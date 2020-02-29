using System;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.Models.UserModels;
using ClassifiedAds.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class UserController : Controller
    {
        private readonly AdsDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserController(AdsDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _dbContext.Set<User>().ToList();
            return View(users);
        }

        public IActionResult Profile(Guid id)
        {
            var user = id != Guid.Empty
                ? _dbContext.Set<User>().FirstOrDefault(x => x.Id == id)
                : new User();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User model)
        {
            User user;
            if (model.Id != Guid.Empty)
            {
                user = _dbContext.Set<User>().FirstOrDefault(x => x.Id == model.Id);
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

        public IActionResult ChangePassword(Guid id)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(x => x.Id == id);
            return View(ChangePasswordModel.FromEntity(user));
        }

        public IActionResult Delete(Guid id)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(x => x.Id == id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(User model)
        {
            var user = _dbContext.Set<User>().FirstOrDefault(x => x.Id == model.Id);
            _dbContext.Set<User>().Remove(user);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
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

        public IActionResult Claims(Guid id)
        {
            var user = _dbContext.Set<User>()
                .Include(x => x.Claims)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

            return View(ClaimsModel.FromEntity(user));
        }

        [HttpPost]
        public IActionResult Claims(ClaimModel model)
        {
            var user = _dbContext.Set<User>()
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == model.User.Id);

            user.Claims.Add(new UserClaim
            {
                Type = model.Type,
                Value = model.Value,
            });

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Claims), new { id = user.Id });
        }

        public IActionResult DeleteClaim(Guid id)
        {
            var claim = _dbContext.Set<UserClaim>()
                .Include(x => x.User)
                .FirstOrDefault(x => x.Id == id);

            return View(ClaimModel.FromEntity(claim));
        }

        [HttpPost]
        public IActionResult DeleteClaim(ClaimModel model)
        {
            var user = _dbContext.Set<User>()
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == model.User.Id);

            var claim = user.Claims.FirstOrDefault(x => x.Id == model.Id);

            user.Claims.Remove(claim);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Claims), new { id = user.Id });
        }

        public IActionResult Roles(Guid id)
        {
            var user = _dbContext.Set<User>()
                .Include("UserRoles.Role")
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefault();

            var roles = _dbContext.Set<Role>().AsNoTracking().ToList();

            var model = new RolesModel
            {
                User = user,
                UserRoles = user.UserRoles.Select(x => new RoleModel { Role = x.Role, RoleId = x.RoleId }).ToList(),
                Roles = roles.Where(x => !user.UserRoles.Any(y => y.RoleId == x.Id)).ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Roles(RolesModel model)
        {
            var user = _dbContext.Set<User>()
                .Include(x => x.UserRoles)
                .Where(x => x.Id == model.User.Id)
                .FirstOrDefault();

            user.UserRoles.Add(new UserRole
            {
                RoleId = model.Role.RoleId,
            });

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Roles), new { model.User.Id });
        }

        public IActionResult DeleteRole(Guid id, Guid roleId)
        {
            var user = _dbContext.Set<User>()
                .Include("UserRoles.Role")
                .Where(x => x.Id == id)
                .FirstOrDefault();
            var role = user.UserRoles.FirstOrDefault(x => x.RoleId == roleId);
            var model = new RoleModel { User = user, Role = role.Role };

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteRole(RoleModel model)
        {
            var user = _dbContext.Set<User>()
                .Include(x => x.UserRoles)
                .Where(x => x.Id == model.User.Id)
                .FirstOrDefault();

            var role = user.UserRoles.FirstOrDefault(x => x.RoleId == model.Role.Id);

            user.UserRoles.Remove(role);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Roles), new { model.User.Id });
        }
    }
}