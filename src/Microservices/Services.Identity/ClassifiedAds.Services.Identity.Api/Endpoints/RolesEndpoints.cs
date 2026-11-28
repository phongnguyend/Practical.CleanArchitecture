using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Web.MinimalApis;
using ClassifiedAds.Services.Identity.Authorization;
using ClassifiedAds.Services.Identity.Commands.Roles;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Models;
using ClassifiedAds.Services.Identity.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddUserRoleCommand = ClassifiedAds.Services.Identity.Commands.Users.AddRoleCommand;
using RemoveUserRoleCommand = ClassifiedAds.Services.Identity.Commands.Users.DeleteRoleCommand;

namespace ClassifiedAds.Services.Identity.Endpoints;

public class RolesEndpoints
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/roles/{id:guid}/users", GetUsers)
            .WithTags("Roles")
            .RequireAuthorization(Permissions.GetRole, Permissions.GetUsers);

        builder.MapPut("api/roles/{id:guid}/users/{userId:guid}", AssignUser)
            .WithTags("Roles")
            .RequireAuthorization(Permissions.UpdateRole, Permissions.UpdateUser);

        builder.MapDelete("api/roles/{id:guid}/users/{userId:guid}", RemoveUser)
            .WithTags("Roles")
            .RequireAuthorization(Permissions.UpdateRole, Permissions.UpdateUser);

        builder.MapGet("api/roles", Get)
            .WithTags("Roles")
            .WithName("Roles_Get")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.GetRoles)
            .Produces<IEnumerable<Role>>(StatusCodes.Status200OK);

        builder.MapGet("api/roles/{id}", GetById)
            .WithTags("Roles")
            .WithName("Roles_GetById")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.GetRole)
            .Produces<Role>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        builder.MapPost("api/roles", Post)
            .WithTags("Roles")
            .WithName("Roles_Post")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.AddRole)
            .AddEndpointFilter<ModelValidationFilter<RoleModel>>()
            .ProducesValidationProblem()
            .Produces<Role>(StatusCodes.Status201Created);

        builder.MapPut("api/roles/{id}", Put)
            .WithTags("Roles")
            .WithName("Roles_Put")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.UpdateRole)
            .AddEndpointFilter<ModelValidationFilter<RoleModel>>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        builder.MapDelete("api/roles/{id}", Delete)
            .WithTags("Roles")
            .WithName("Roles_Delete")
            .RequireAuthorization()
            .RequireAuthorization(Permissions.DeleteRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetUsers(Guid id, Dispatcher dispatcher)
    {
        var role = await dispatcher.DispatchAsync(new GetRoleQuery { Id = id, IncludeUsers = true, AsNoTracking = true });
        if (role == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(role.UserRoles.Select(link => link.User).OrderBy(user => user.UserName)
            .Select(user => new { user.Id, user.UserName, user.Email }));
    }

    private static Task<IResult> AssignUser(Guid id, Guid userId, Dispatcher dispatcher)
    {
        return SetMembership(id, userId, true, dispatcher);
    }

    private static Task<IResult> RemoveUser(Guid id, Guid userId, Dispatcher dispatcher)
    {
        return SetMembership(id, userId, false, dispatcher);
    }

    private static async Task<IResult> SetMembership(Guid id, Guid userId, bool assigned, Dispatcher dispatcher)
    {
        var role = await dispatcher.DispatchAsync(new GetRoleQuery { Id = id, AsNoTracking = true });
        var user = await dispatcher.DispatchAsync(new GetUserQuery { Id = userId, IncludeUserRoles = true });
        if (role == null || user == null)
        {
            return Results.NotFound();
        }

        var memberships = user.UserRoles.Where(link => link.RoleId == id).ToList();
        if (assigned && memberships.Count == 0)
        {
            await dispatcher.DispatchAsync(new AddUserRoleCommand
            {
                User = user,
                Role = new UserRole { UserId = userId, RoleId = id },
            });
        }
        else if (!assigned)
        {
            foreach (var membership in memberships)
            {
                await dispatcher.DispatchAsync(new RemoveUserRoleCommand { User = user, Role = membership });
            }
        }

        return Results.NoContent();
    }

    private static async Task<IResult> Get([FromServices] Dispatcher dispatcher)
    {
        var roles = await dispatcher.DispatchAsync(new GetRolesQuery { AsNoTracking = true });
        var model = roles.ToModels();
        return Results.Ok(model);
    }

    private static async Task<IResult> GetById(Guid id,
        [FromServices] Dispatcher dispatcher)
    {
        var role = await dispatcher.DispatchAsync(new GetRoleQuery { Id = id, AsNoTracking = true });
        var model = role.ToModel();
        return Results.Ok(model);
    }

    private static async Task<IResult> Post([FromBody] RoleModel model,
        [FromServices] Dispatcher dispatcher)
    {
        var role = new Role
        {
            Name = model.Name,
            NormalizedName = model.Name.ToUpper(),
        };

        await dispatcher.DispatchAsync(new AddUpdateRoleCommand { Role = role });

        model = role.ToModel();

        return Results.Created($"/api/roles/{model.Id}", model);
    }

    private static async Task<IResult> Put(Guid id, [FromBody] RoleModel model,
        [FromServices] Dispatcher dispatcher)
    {
        var role = await dispatcher.DispatchAsync(new GetRoleQuery { Id = id });

        role.Name = model.Name;
        role.NormalizedName = model.Name.ToUpper();

        await dispatcher.DispatchAsync(new AddUpdateRoleCommand { Role = role });

        model = role.ToModel();

        return Results.Ok(model);
    }

    private static async Task<IResult> Delete(Guid id,
        [FromServices] Dispatcher dispatcher)
    {
        var role = await dispatcher.DispatchAsync(new GetRoleQuery { Id = id });
        await dispatcher.DispatchAsync(new DeleteRoleCommand { Role = role });

        return Results.Ok();
    }
}
