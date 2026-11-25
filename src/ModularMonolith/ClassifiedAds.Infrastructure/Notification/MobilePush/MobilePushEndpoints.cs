using ClassifiedAds.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public sealed record MobilePushDeviceRegistration(string App, string Platform, string Token);

public static class MobilePushEndpoints
{
    private static string UserId(ClaimsPrincipal user) => user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

    public static void Map(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/notifications").RequireAuthorization().RequireRateLimiting("PushNotifications").WithTags("Notifications");
        group.MapGet("/status", (IOptions<MobilePushOptions> options) => Results.Ok(new { enabled = options.Value.UsedFirebase(), allowTestMessages = options.Value.UsedFirebase() && options.Value.AllowTestMessages }));
        group.MapPut("/devices/{id:guid}", Register);
        group.MapDelete("/devices/{id:guid}", async (Guid id, ClaimsPrincipal user, MobilePushStore db, IOptions<MobilePushOptions> options, CancellationToken ct) =>
        {
            if (!options.Value.UsedFirebase())
            {
                return Results.NoContent();
            }
            var subject = UserId(user);
            if (string.IsNullOrEmpty(subject))
            {
                return Results.Forbid();
            }
            await db.Devices.Where(x => x.Id == id && x.UserId == subject).ExecuteDeleteAsync(ct);
            return Results.NoContent();
        });
        group.MapGet("/inbox", async (ClaimsPrincipal user, MobilePushStore db, IOptions<MobilePushOptions> options, int? page, CancellationToken ct) =>
        {
            var subject = UserId(user);
            if (string.IsNullOrEmpty(subject))
            {
                return Results.Forbid();
            }
            var query = db.Messages.AsNoTracking().Where(x => x.UserId == subject);
            var total = await query.CountAsync(ct);
            var items = await query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Skip((Math.Clamp(page ?? 1, 1, 10000) - 1) * 20)
                .Take(20).Select(x => new { x.Id, x.Title, x.Body, x.Section, x.CreatedAt, x.ReadAt }).ToListAsync(ct);
            return Results.Ok(new { items, total });
        });
        group.MapPut("/inbox/{id:guid}/read", async (Guid id, ClaimsPrincipal user, MobilePushStore db, IOptions<MobilePushOptions> options, CancellationToken ct) =>
        {
            var subject = UserId(user);
            if (string.IsNullOrEmpty(subject))
            {
                return Results.Forbid();
            }
            var count = await db.Messages.Where(x => x.Id == id && x.UserId == subject).ExecuteUpdateAsync(x => x.SetProperty(m => m.ReadAt, m => m.ReadAt ?? DateTime.UtcNow), ct);
            return count == 0 ? Results.NotFound() : Results.NoContent();
        });
        group.MapPost("/test", async (ClaimsPrincipal user, IOptions<MobilePushOptions> options, IMobilePushNotificationService notifications, CancellationToken ct) =>
        {
            if (!options.Value.UsedFirebase() || !options.Value.AllowTestMessages)
            {
                return Results.NotFound();
            }
            var subject = UserId(user);
            if (string.IsNullOrEmpty(subject))
            {
                return Results.Forbid();
            }
            var id = await notifications.EnqueueAsync(subject, "Push notifications are working", "This notification was requested from your mobile app.", cancellationToken: ct);
            return Results.Accepted(value: new { id });
        });
    }

    public static async Task<IResult> Register(Guid id, MobilePushDeviceRegistration request, ClaimsPrincipal user, MobilePushStore db, IOptions<MobilePushOptions> options, CancellationToken ct)
    {
        if (!options.Value.UsedFirebase())
        {
            return Results.Problem("Push notifications are not configured.", statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        var subject = UserId(user);
        if (string.IsNullOrEmpty(subject) || subject.Length > 200)
        {
            return Results.Forbid();
        }
        var apps = new[] { "Kotlin", "Flutter", "ReactNative", "MAUI", "Capacitor" };
        if (id == Guid.Empty || !apps.Contains(request.App, StringComparer.Ordinal) || (request.Platform != "android" && request.Platform != "ios")
            || string.IsNullOrWhiteSpace(request.Token) || request.Token.Length > 2048 || request.Token.Any(char.IsWhiteSpace))
        {
            return Results.BadRequest(new { message = "Invalid device registration." });
        }
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var device = await db.Devices.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (device != null && device.UserId != subject)
        {
            return Results.Conflict(new { message = "Use a new installation registration for this account." });
        }
        if (device == null && await db.Devices.CountAsync(x => x.UserId == subject, ct) >= 20)
        {
            return Results.Problem("Device limit reached.", statusCode: StatusCodes.Status409Conflict);
        }
        // A rotated/reused token must not continue receiving a previous account's notifications.
        await db.Devices.Where(x => x.Token == request.Token && x.Id != id).ExecuteDeleteAsync(ct);
        if (device == null)
        {
            device = new MobilePushDevice { Id = id, UserId = subject };
            db.Devices.Add(device);
        }
        device.App = request.App;
        device.Platform = request.Platform;
        device.Token = request.Token;
        device.TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));
        device.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return Results.NoContent();
    }
}
