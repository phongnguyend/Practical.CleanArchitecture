using ClassifiedAds.Domain.Entities;
using ClassifiedAds.IdentityServer.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
