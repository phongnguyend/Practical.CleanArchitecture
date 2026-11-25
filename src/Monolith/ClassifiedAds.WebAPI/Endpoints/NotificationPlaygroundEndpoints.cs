using ClassifiedAds.Infrastructure.Notification.MobilePush;
using Microsoft.Extensions.Options;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Endpoints;

public sealed record QueueMobilePushRequest(string Title, string Body, string Section);

public sealed record QueueEmailRequest(string From, string Recipient, string Subject, string Body);

public sealed record QueueSmsRequest(string Recipient, string Body);

public static class NotificationPlaygroundEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var mobilePush = app.MapGroup("/api/notifications").RequireAuthorization().RequireRateLimiting("PushNotifications").WithTags("Notification playgrounds");
        mobilePush.MapPost("/playground", QueueMobilePush);
        mobilePush.MapGet("/playground", async (ClaimsPrincipal user, MobilePushStore db, IOptions<MobilePushOptions> options, CancellationToken ct) =>
        {
            var subject = UserId(user);
            if (string.IsNullOrEmpty(subject))
            {
                return Results.Forbid();
            }

            var deviceCount = await db.Devices.CountAsync(x => x.UserId == subject, ct);
            var items = await db.Messages.AsNoTracking().Where(x => x.UserId == subject)
                .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Take(20)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Body,
                    x.Section,
                    x.CreatedAt,
                    Deliveries = db.Deliveries.Count(d => d.MessageId == x.Id),
                    Completed = db.Deliveries.Count(d => d.MessageId == x.Id && d.CompletedAt != null && d.LastError == null),
                    Failed = db.Deliveries.Count(d => d.MessageId == x.Id && d.CompletedAt != null && d.LastError != null),
                    Pending = db.Deliveries.Count(d => d.MessageId == x.Id && d.CompletedAt == null),
                }).ToListAsync(ct);
            return Results.Ok(new { provider = options.Value.Provider, deviceCount, items });
        });
        var group = app.MapGroup("/api/notifications/playgrounds").RequireAuthorization().RequireRateLimiting("PushNotifications").WithTags("Notification playgrounds");
        group.MapPost("/email", QueueEmail);
        group.MapPost("/sms", QueueSms);
        group.MapGet("/email", async (ClaimsPrincipal user, AdsDbContext db, IConfiguration configuration, CancellationToken ct) =>
        {
            var subject = UserId(user);
            if (string.IsNullOrWhiteSpace(subject))
            {
                return Results.Forbid();
            }

            var items = await db.Set<EmailMessage>().AsNoTracking()
                .OrderByDescending(x => x.CreatedDateTime).ThenBy(x => x.Id).Take(20)
                .Select(x => new { x.Id, title = x.Subject, body = x.Body, recipient = x.Tos, x.From, createdAt = x.CreatedDateTime, sentAt = x.SentDateTime, attempts = x.AttemptCount, status = x.SentDateTime != null ? "Sent" : x.AttemptCount >= (x.MaxAttemptCount == 0 ? 5 : x.MaxAttemptCount) ? "Failed" : x.ExpiredDateTime < DateTimeOffset.UtcNow ? "Expired" : "Pending" }).ToListAsync(ct);
            return Results.Ok(new { provider = configuration["Notification:Email:Provider"] ?? "Not configured", items });
        });
        group.MapGet("/sms", async (ClaimsPrincipal user, AdsDbContext db, IConfiguration configuration, CancellationToken ct) =>
        {
            var subject = UserId(user);
            if (string.IsNullOrWhiteSpace(subject))
            {
                return Results.Forbid();
            }

            var items = await db.Set<SmsMessage>().AsNoTracking()
                .OrderByDescending(x => x.CreatedDateTime).ThenBy(x => x.Id).Take(20)
                .Select(x => new { x.Id, title = "SMS", body = x.Message, recipient = x.PhoneNumber, createdAt = x.CreatedDateTime, sentAt = x.SentDateTime, attempts = x.AttemptCount, status = x.SentDateTime != null ? "Sent" : x.AttemptCount >= (x.MaxAttemptCount == 0 ? 5 : x.MaxAttemptCount) ? "Failed" : x.ExpiredDateTime < DateTimeOffset.UtcNow ? "Expired" : "Pending" }).ToListAsync(ct);
            return Results.Ok(new { provider = configuration["Notification:Sms:Provider"] ?? "Not configured", items });
        });
    }

    public static async Task<IResult> QueueMobilePush(QueueMobilePushRequest request, ClaimsPrincipal user, IMobilePushNotificationService notifications, CancellationToken ct)
    {
        var subject = UserId(user);
        if (string.IsNullOrWhiteSpace(subject) || subject.Length > 200)
        {
            return Results.Forbid();
        }

        var title = request.Title?.Trim();
        var body = request.Body?.Trim();
        if (string.IsNullOrWhiteSpace(title) || title.Length > 100 || string.IsNullOrWhiteSpace(body) || body.Length > 500 || !MobilePushNotificationService.Sections.Contains(request.Section, StringComparer.Ordinal))
        {
            return Results.BadRequest(new { message = "Enter a title (1–100 characters), message (1–500 characters), and a supported destination." });
        }

        // Recipient comes only from the authenticated principal, never from request data.
        var id = await notifications.EnqueueAsync(subject, title, body, request.Section, ct);
        return Results.Accepted(value: new { id });
    }

    private static string UserId(ClaimsPrincipal user) => user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

    public static async Task<IResult> QueueEmail(QueueEmailRequest request, ClaimsPrincipal user, AdsDbContext db, CancellationToken ct)
    {
        var subject = UserId(user);
        if (string.IsNullOrWhiteSpace(subject) || subject.Length > 200)
        {
            return Results.Forbid();
        }

        if (!ValidEmail(request.From) || !ValidEmail(request.Recipient) || string.IsNullOrWhiteSpace(request.Subject) || request.Subject.Trim().Length > 200 || request.Subject.Contains('\r') || request.Subject.Contains('\n') || string.IsNullOrWhiteSpace(request.Body) || request.Body.Trim().Length > 10000)
        {
            return Results.BadRequest(new { message = "Enter valid From and To addresses, a subject (1-200 characters), and a message (1-10000 characters)." });
        }

        var message = new EmailMessage
        {
            Id = Guid.NewGuid(),
            From = request.From.Trim(),
            Tos = request.Recipient.Trim(),
            Subject = request.Subject.Trim(),
            Body = request.Body.Trim(),
            CreatedDateTime = DateTimeOffset.UtcNow,
            MaxAttemptCount = 5,
            ExpiredDateTime = DateTimeOffset.UtcNow.AddDays(1),
        };
        db.Set<EmailMessage>().Add(message);
        await db.SaveChangesAsync(ct);
        return Results.Accepted(value: new { id = message.Id });
    }

    public static async Task<IResult> QueueSms(QueueSmsRequest request, ClaimsPrincipal user, AdsDbContext db, CancellationToken ct)
    {
        var subject = UserId(user);
        if (string.IsNullOrWhiteSpace(subject) || subject.Length > 200)
        {
            return Results.Forbid();
        }

        if (string.IsNullOrWhiteSpace(request.Recipient) || !Regex.IsMatch(request.Recipient.Trim(), @"^\+[1-9][0-9]{7,14}$") || string.IsNullOrWhiteSpace(request.Body) || request.Body.Trim().Length > 1600)
        {
            return Results.BadRequest(new { message = "Enter a phone number in international format (+country code) and a message (1-1600 characters)." });
        }

        var message = new SmsMessage
        {
            Id = Guid.NewGuid(),
            PhoneNumber = request.Recipient.Trim(),
            Message = request.Body.Trim(),
            CreatedDateTime = DateTimeOffset.UtcNow,
            MaxAttemptCount = 5,
            ExpiredDateTime = DateTimeOffset.UtcNow.AddDays(1),
        };
        db.Set<SmsMessage>().Add(message);
        await db.SaveChangesAsync(ct);
        return Results.Accepted(value: new { id = message.Id });
    }

    private static bool ValidEmail(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Length <= 254 && !value.Contains('\r') && !value.Contains('\n')
            && MailAddress.TryCreate(value.Trim(), out var address) && address.Address == value.Trim();
    }
}
