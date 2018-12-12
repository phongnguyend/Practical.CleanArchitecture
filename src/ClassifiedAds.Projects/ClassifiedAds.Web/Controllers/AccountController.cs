using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Web.Models;
using ClassifiedAds.Contracts.DTOs;
using Microsoft.Extensions.Configuration;
using ClassifiedAds.Infrastructure;

namespace ClassifiedAds.Web.Controllers
{
    public class AccountController : Controller
    {
        private IConfiguration _configuration;
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public AccountController(IConfiguration configuration, UserManager<User> userManager,
                                 SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Username };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (createResult.Succeeded)
                {
                    //TODO: Create and store an Email into database
                    Guid emailId = Guid.NewGuid();//set this equal to the Id of the created Email 
                    var queueConfig = _configuration.GetSection("QueueConfiguration");
                    var msSender = new MessageQueueSender<QueueItem>(queueConfig["HostName"], queueConfig["UserName"], queueConfig["Password"]);
                    msSender.Send(new QueueItem { Id = emailId }, queueConfig["Exchange"], queueConfig["RoutingKey"]);

                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _signInManager.PasswordSignInAsync(
                                            model.Username, model.Password,
                                            model.RememberMe, false);
                if (loginResult.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Could not login");
            return View(model);
        }

    }
}