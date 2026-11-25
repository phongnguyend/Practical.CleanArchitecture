using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence;
using ClassifiedAds.WebAPI.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.UnitTests.Notification;

public sealed class NotificationPlaygroundTests
{
    private static readonly ClaimsPrincipal User = new(new ClaimsIdentity(new[] { new Claim("sub", "test-user") }, "test"));

    [Fact]
    public async Task EmailIsPersistedInExistingQueueWithRetryDefaults()
    {
        using var fixture = new Fixture();
        var result = await NotificationPlaygroundEndpoints.QueueEmail(new QueueEmailRequest(" sender@example.com ", " recipient@example.com ", " Subject ", " Message "), User, fixture.Context, CancellationToken.None);
        Assert.Equal(202, ((IStatusCodeHttpResult)result).StatusCode);
        var row = await fixture.Context.Set<EmailMessage>().SingleAsync();
        Assert.Equal("sender@example.com", row.From);
        Assert.Equal("recipient@example.com", row.Tos);
        Assert.Equal("Subject", row.Subject);
        Assert.Equal("Message", row.Body);
        Assert.Equal(5, row.MaxAttemptCount);
        Assert.Equal(0, row.AttemptCount);
        Assert.Null(row.SentDateTime);
        Assert.True(row.ExpiredDateTime > row.CreatedDateTime);
    }

    [Fact]
    public async Task SmsIsPersistedInExistingQueueWithoutSending()
    {
        using var fixture = new Fixture();
        var result = await NotificationPlaygroundEndpoints.QueueSms(new QueueSmsRequest(" +15551234567 ", " Message "), User, fixture.Context, CancellationToken.None);
        Assert.Equal(202, ((IStatusCodeHttpResult)result).StatusCode);
        var row = await fixture.Context.Set<SmsMessage>().SingleAsync();
        Assert.Equal("+15551234567", row.PhoneNumber);
        Assert.Equal("Message", row.Message);
        Assert.Equal(5, row.MaxAttemptCount);
        Assert.Null(row.SentDateTime);
    }

    [Theory]
    [InlineData("invalid", "to@example.com", "Subject", "Body")]
    [InlineData("from@example.com", "a@example.com;b@example.com", "Subject", "Body")]
    [InlineData("from@example.com", "to@example.com", "Subject\r\nBcc: other@example.com", "Body")]
    [InlineData("from@example.com", "to@example.com", " ", "Body")]
    [InlineData("from@example.com", "to@example.com", "Subject", " ")]
    public async Task InvalidEmailDoesNotEnterQueue(string from, string to, string subject, string body)
    {
        using var fixture = new Fixture();
        var result = await NotificationPlaygroundEndpoints.QueueEmail(new QueueEmailRequest(from, to, subject, body), User, fixture.Context, CancellationToken.None);
        Assert.Equal(400, ((IStatusCodeHttpResult)result).StatusCode);
        Assert.Empty(await fixture.Context.Set<EmailMessage>().ToListAsync());
    }

    [Theory]
    [InlineData("5551234567", "Body")]
    [InlineData("+0123456789", "Body")]
    [InlineData("+15551234567", " ")]
    [InlineData("+15551234567; +15551234568", "Body")]
    public async Task InvalidSmsDoesNotEnterQueue(string recipient, string body)
    {
        using var fixture = new Fixture();
        var result = await NotificationPlaygroundEndpoints.QueueSms(new QueueSmsRequest(recipient, body), User, fixture.Context, CancellationToken.None);
        Assert.Equal(400, ((IStatusCodeHttpResult)result).StatusCode);
        Assert.Empty(await fixture.Context.Set<SmsMessage>().ToListAsync());
    }

    [Fact]
    public async Task BothChannelsRejectOversizedContentAndMissingIdentity()
    {
        using var fixture = new Fixture();
        var email = new QueueEmailRequest("from@example.com", "to@example.com", "Subject", new string('x', 10001));
        var sms = new QueueSmsRequest("+15551234567", new string('x', 1601));
        Assert.Equal(400, ((IStatusCodeHttpResult)await NotificationPlaygroundEndpoints.QueueEmail(email, User, fixture.Context, CancellationToken.None)).StatusCode);
        Assert.Equal(400, ((IStatusCodeHttpResult)await NotificationPlaygroundEndpoints.QueueSms(sms, User, fixture.Context, CancellationToken.None)).StatusCode);
        Assert.IsType<ForbidHttpResult>(await NotificationPlaygroundEndpoints.QueueEmail(email, new ClaimsPrincipal(), fixture.Context, CancellationToken.None));
        Assert.IsType<ForbidHttpResult>(await NotificationPlaygroundEndpoints.QueueSms(sms, new ClaimsPrincipal(), fixture.Context, CancellationToken.None));
    }

    private sealed class Fixture : IDisposable
    {
        private readonly SqliteConnection connection = new("Data Source=:memory:");

        public QueueContext Context { get; }

        public Fixture()
        {
            connection.Open();
            Context = new QueueContext(new DbContextOptionsBuilder<AdsDbContext>().UseSqlite(connection).Options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
            connection.Dispose();
        }
    }

    private sealed class QueueContext : AdsDbContext
    {
        public QueueContext(DbContextOptions<AdsDbContext> options) : base(options, NullLogger<AdsDbContext>.Instance)
        {
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<EmailMessage>().ToTable("EmailMessages").Ignore(x => x.EmailMessageAttachments);
            model.Entity<SmsMessage>().ToTable("SmsMessages");
        }
    }
}
