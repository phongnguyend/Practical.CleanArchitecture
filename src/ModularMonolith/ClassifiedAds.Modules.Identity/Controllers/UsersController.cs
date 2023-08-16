using ClassifiedAds.Application;
using ClassifiedAds.Contracts.Notification.DTOs;
using ClassifiedAds.Contracts.Notification.Services;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Modules.Identity.Authorization;
using ClassifiedAds.Modules.Identity.Commands.Users;
using ClassifiedAds.Modules.Identity.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Models;
using ClassifiedAds.Modules.Identity.Queries.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace ClassifiedAds.Modules.Identity.Controllers;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly Dispatcher _dispatcher;
    private readonly UserManager<User> _userManager;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailMessageService _emailMessageService;
    private readonly IdentityModuleOptions _moduleOptions;

    public UsersController(Dispatcher dispatcher,
        UserManager<User> userManager,
        ILogger<UsersController> logger,
        IDateTimeProvider dateTimeProvider,
        IEmailMessageService emailMessageService,
        IOptionsSnapshot<IdentityModuleOptions> moduleOptions)
    {
        _dispatcher = dispatcher;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        _emailMessageService = emailMessageService;
        _moduleOptions = moduleOptions.Value;
    }

    [Authorize(AuthorizationPolicyNames.GetUsersPolicy)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        var users = await _dispatcher.DispatchAsync(new GetUsersQuery());
        var model = users.ToModels();
        return Ok(model);
    }

    [Authorize(AuthorizationPolicyNames.GetUserPolicy)]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> Get(Guid id)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id, AsNoTracking = true });
        var model = user.ToModel();
        return Ok(model);
    }

    [Authorize(AuthorizationPolicyNames.AddUserPolicy)]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<User>> Post([FromBody] UserModel model)
    {
        User user = new User
        {
            UserName = model.UserName,
            NormalizedUserName = model.UserName.ToUpper(),
            Email = model.Email,
            NormalizedEmail = model.Email.ToUpper(),
            EmailConfirmed = model.EmailConfirmed,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberConfirmed = model.PhoneNumberConfirmed,
            TwoFactorEnabled = model.TwoFactorEnabled,
            LockoutEnabled = model.LockoutEnabled,
            LockoutEnd = model.LockoutEnd,
            AccessFailedCount = model.AccessFailedCount,
        };

        _ = await _userManager.CreateAsync(user);

        model = user.ToModel();
        return Created($"/api/users/{model.Id}", model);
    }

    [Authorize(AuthorizationPolicyNames.UpdateUserPolicy)]
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Put(Guid id, [FromBody] UserModel model)
    {
        User user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id });

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

        _ = await _userManager.UpdateAsync(user);

        model = user.ToModel();
        return Ok(model);
    }

    [Authorize(AuthorizationPolicyNames.SetPasswordPolicy)]
    [HttpPut("{id}/password")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetPassword(Guid id, [FromBody] SetPasswordModel model)
    {
        User user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var rs = await _userManager.ResetPasswordAsync(user, token, model.Password);

        if (rs.Succeeded)
        {
            return Ok();
        }

        return BadRequest(rs.Errors);
    }

    [Authorize(AuthorizationPolicyNames.DeleteUserPolicy)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id });
        await _dispatcher.DispatchAsync(new DeleteUserCommand { User = user });

        return Ok();
    }

    [Authorize(AuthorizationPolicyNames.SendResetPasswordEmailPolicy)]
    [HttpPost("{id}/passwordresetemail")]
    public async Task<ActionResult> SendResetPasswordEmail(Guid id)
    {
        User user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id });

        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"{_moduleOptions.IdentityServerAuthentication.Authority}/Account/ResetPassword?token={HttpUtility.UrlEncode(token)}&email={user.Email}";

            await _emailMessageService.CreateEmailMessageAsync(new EmailMessageDTO
            {
                From = "phong@gmail.com",
                Tos = user.Email,
                Subject = "Forgot Password",
                Body = string.Format("Reset Url: {0}", resetUrl),
            });
        }
        else
        {
            // email user and inform them that they do not have an account
        }

        return Ok();
    }

    [Authorize(AuthorizationPolicyNames.SendConfirmationEmailAddressEmailPolicy)]
    [HttpPost("{id}/emailaddressconfirmation")]
    public async Task<ActionResult> SendConfirmationEmailAddressEmail(Guid id)
    {
        User user = await _dispatcher.DispatchAsync(new GetUserQuery { Id = id });

        if (user != null)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationEmail = $"{_moduleOptions.IdentityServerAuthentication.Authority}/Account/ConfirmEmailAddress?token={HttpUtility.UrlEncode(token)}&email={user.Email}";

            await _emailMessageService.CreateEmailMessageAsync(new EmailMessageDTO
            {
                From = "phong@gmail.com",
                Tos = user.Email,
                Subject = "Confirmation Email",
                Body = string.Format("Confirmation Email: {0}", confirmationEmail),
            });
        }
        else
        {
            // email user and inform them that they do not have an account
        }

        return Ok();
    }
}