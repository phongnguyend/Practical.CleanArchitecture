using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Identity.Authorization;
using ClassifiedAds.Services.Identity.Commands.EmailMessages;
using ClassifiedAds.Services.Identity.Commands.Users;
using ClassifiedAds.Services.Identity.DTOs;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Models;
using ClassifiedAds.Services.Identity.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ClassifiedAds.Services.Identity.Endpoints;

public class UsersEndpoints
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/users/{id:guid}/roles", GetRoles)
            .WithTags("Users")
            .RequireAuthorization(Permissions.GetUser, Permissions.GetRoles);

        builder.MapGet("api/users", Get)
            .WithTags("Users")
            .WithName("Users_Get")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.GetUsers)
            .Produces<IEnumerable<User>>(StatusCodes.Status200OK);

        builder.MapGet("api/users/{id}", GetById)
            .WithTags("Users")
            .WithName("Users_GetById")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.GetUser)
            .Produces<User>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        builder.MapPost("api/users", Post)
            .WithTags("Users")
            .WithName("Users_Post")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.AddUser)
            .AddEndpointFilter<ModelValidationFilter<UserModel>>()
            .ProducesValidationProblem()
            .Produces<User>(StatusCodes.Status201Created);

        builder.MapPut("api/users/{id}", Put)
            .WithTags("Users")
            .WithName("Users_Put")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.UpdateUser)
            .AddEndpointFilter<ModelValidationFilter<UserModel>>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        builder.MapPut("api/users/{id}/password", SetPassword)
            .WithTags("Users")
            .WithName("Users_SetPassword")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.SetPassword)
            .AddEndpointFilter<ModelValidationFilter<SetPasswordModel>>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        builder.MapDelete("api/users/{id}", Delete)
            .WithTags("Users")
            .WithName("Users_Delete")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.DeleteUser)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        builder.MapPost("api/users/{id}/passwordresetemail", SendResetPasswordEmail)
            .WithTags("Users")
            .WithName("Users_SendResetPasswordEmail")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.SendResetPasswordEmail)
            .Produces(StatusCodes.Status200OK);

        builder.MapPost("api/users/{id}/emailaddressconfirmation", SendConfirmationEmailAddressEmail)
            .WithTags("Users")
            .WithName("Users_SendConfirmationEmailAddressEmail")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.SendConfirmationEmailAddressEmail)
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetRoles(Guid id, Dispatcher dispatcher)
    {
        var user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id, IncludeRoles = true, AsNoTracking = true });
        if (user == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user.UserRoles.OrderBy(link => link.Role.Name).Select(link => new { link.Role.Id, link.Role.Name }));
    }

    private static async Task<IResult> Get([FromServices] Dispatcher dispatcher)
    {
        var users = await dispatcher.DispatchAsync(new GetUsersQuery());
        var model = users.ToModels();
        return Results.Ok(model);
    }

    private static async Task<IResult> GetById(Guid id,
        [FromServices] Dispatcher dispatcher)
    {
        var user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id, AsNoTracking = true });
        var model = user.ToModel();
        return Results.Ok(model);
    }

    private static async Task<IResult> Post([FromBody] UserModel model,
        [FromServices] UserManager<User> userManager)
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

        _ = await userManager.CreateAsync(user);

        model = user.ToModel();
        return Results.Created($"/api/users/{model.Id}", model);
    }

    private static async Task<IResult> Put(Guid id, [FromBody] UserModel model,
        [FromServices] Dispatcher dispatcher,
        [FromServices] UserManager<User> userManager)
    {
        User user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id });

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

        _ = await userManager.UpdateAsync(user);

        model = user.ToModel();
        return Results.Ok(model);
    }

    private static async Task<IResult> SetPassword(Guid id, [FromBody] SetPasswordModel model,
        [FromServices] Dispatcher dispatcher,
        [FromServices] UserManager<User> userManager)
    {
        User user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id });

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var rs = await userManager.ResetPasswordAsync(user, token, model.Password);

        if (rs.Succeeded)
        {
            return Results.Ok();
        }

        return Results.BadRequest(rs.Errors);
    }

    private static async Task<IResult> Delete(Guid id,
        [FromServices] Dispatcher dispatcher)
    {
        var user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id });
        await dispatcher.DispatchAsync(new DeleteUserCommand { User = user });

        return Results.Ok();
    }

    private static async Task<IResult> SendResetPasswordEmail(Guid id,
        [FromServices] Dispatcher dispatcher,
        [FromServices] UserManager<User> userManager,
        [FromServices] IConfiguration configuration)
    {
        User user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id });

        if (user != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"{configuration["Authentication:IdentityServer:Authority"]}/Account/ResetPassword?token={HttpUtility.UrlEncode(token)}&email={user.Email}";

            await dispatcher.DispatchAsync(new AddEmailMessageCommand
            {
                EmailMessage = new EmailMessageDTO
                {
                    From = "phong@gmail.com",
                    Tos = user.Email,
                    Subject = "Forgot Password",
                    Body = string.Format("Reset Url: {0}", resetUrl),
                },
            });
        }
        else
        {
            // email user and inform them that they do not have an account
        }

        return Results.Ok();
    }

    private static async Task<IResult> SendConfirmationEmailAddressEmail(Guid id,
        [FromServices] Dispatcher dispatcher,
        [FromServices] UserManager<User> userManager,
        [FromServices] IConfiguration configuration)
    {
        User user = await dispatcher.DispatchAsync(new GetUserQuery { Id = id });

        if (user != null)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationEmail = $"{configuration["Authentication:IdentityServer:Authority"]}/Account/ConfirmEmailAddress?token={HttpUtility.UrlEncode(token)}&email={user.Email}";

            await dispatcher.DispatchAsync(new AddEmailMessageCommand
            {
                EmailMessage = new EmailMessageDTO
                {
                    From = "phong@gmail.com",
                    Tos = user.Email,
                    Subject = "Confirmation Email",
                    Body = string.Format("Confirmation Email: {0}", confirmationEmail),
                },
            });
        }
        else
        {
            // email user and inform them that they do not have an account
        }

        return Results.Ok();
    }
}
