using ClassifiedAds.WebAPI.Endpoints;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Notification.MobilePush;
using ClassifiedAds.Persistence;
using ClassifiedAds.Persistence.DbConfigurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.UnitTests.Notification;

public sealed class MobilePushNotificationTests
{
    private static readonly IOptions<MobilePushOptions> Enabled = Options.Create(new MobilePushOptions { Provider = "Firebase", Firebase = new FirebaseMobilePushOptions { ProjectId = "test-project" } });

    [Fact]
    public void FakeProviderIsTheDefaultAndNeedsNoCredentials()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMobilePushNotifications<AdsDbContext>(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider();
        Assert.IsType<FakeMobilePushSender>(provider.GetRequiredService<IMobilePushSender>());
        Assert.True(provider.GetRequiredService<IOptions<MobilePushOptions>>().Value.UsedFake());
    }

    [Theory]
    [InlineData("Unsupported")]
    [InlineData("Firebase")]
    public void InvalidProviderConfigurationFailsAtStartup(string provider)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { ["Provider"] = provider }).Build();
        Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddMobilePushNotifications<AdsDbContext>(configuration));
    }

    [Fact]
    public async Task FakeProviderPersistsWithoutContactingFirebase()
    {
        using var fixture = new StoreFixture();
        var service = new MobilePushNotificationService(fixture.Store, Options.Create(new MobilePushOptions()));
        var id = await service.EnqueueAsync("alice", "Title", "Body");
        Assert.Equal(id, (await fixture.Store.Messages.SingleAsync()).Id);
        Assert.Empty(await fixture.Store.Deliveries.ToListAsync());
        await new FakeMobilePushSender().SendAsync(null, null, CancellationToken.None);
    }

    [Fact]
    public void PushEntitiesShareAdsContextWithEmailAndSms()
    {
        using var context = new AdsDbContext(new DbContextOptionsBuilder<AdsDbContext>().UseSqlServer("Server=unused;Database=unused").Options, NullLogger<AdsDbContext>.Instance);
        Assert.NotNull(context.Model.FindEntityType(typeof(EmailMessage)));
        Assert.NotNull(context.Model.FindEntityType(typeof(SmsMessage)));
        Assert.NotNull(context.Model.FindEntityType(typeof(MobilePushDevice)));
        Assert.NotNull(context.Model.FindEntityType(typeof(MobilePushMessage)));
        Assert.NotNull(context.Model.FindEntityType(typeof(MobilePushDelivery)));
    }

    [Fact]
    public async Task RegistrationCannotOverwriteAnotherUsersInstallation()
    {
        using var fixture = new StoreFixture();
        var id = Guid.NewGuid();
        Assert.Equal(204, Status(await Register(fixture.Store, id, "alice", "token-a")));
        Assert.Equal(409, Status(await Register(fixture.Store, id, "bob", "token-b")));
        Assert.Equal("alice", (await fixture.Store.Devices.SingleAsync()).UserId);
    }

    [Fact]
    public async Task ReusedTokenStopsPreviousAccountDelivery()
    {
        using var fixture = new StoreFixture();
        var old = Guid.NewGuid();
        await Register(fixture.Store, old, "alice", "shared-token");
        await new MobilePushNotificationService(fixture.Store, Enabled).EnqueueAsync("alice", "Private", "Old account");
        await Register(fixture.Store, Guid.NewGuid(), "bob", "shared-token");
        var sender = new RecordingSender();
        await MobilePushDeliveryWorker.DeliverAsync(fixture.Store, sender, CancellationToken.None);
        Assert.Empty(sender.Recipients);
        Assert.Equal("bob", (await fixture.Store.Devices.AsNoTracking().SingleAsync()).UserId);
    }

    [Fact]
    public async Task InboxAndDeliveryQueueAreScopedToRecipient()
    {
        using var fixture = new StoreFixture();
        await Register(fixture.Store, Guid.NewGuid(), "alice", "token-a");
        await Register(fixture.Store, Guid.NewGuid(), "bob", "token-b");
        var id = await new MobilePushNotificationService(fixture.Store, Enabled).EnqueueAsync("alice", "Update", "Details", "products");
        Assert.Equal(id, (await fixture.Store.Messages.SingleAsync()).Id);
        Assert.Equal("alice", (await fixture.Store.Deliveries.SingleAsync()).UserId);
        var sender = new RecordingSender();
        await MobilePushDeliveryWorker.DeliverAsync(fixture.Store, sender, CancellationToken.None);
        Assert.Equal(new[] { "alice" }, sender.Recipients);
        Assert.NotNull((await fixture.Store.Deliveries.AsNoTracking().SingleAsync()).CompletedAt);
        await MobilePushDeliveryWorker.DeliverAsync(fixture.Store, sender, CancellationToken.None);
        Assert.Single(sender.Recipients);
    }

    [Fact]
    public async Task TransientFailureIsRetriedWithoutRecordingSensitiveExceptionText()
    {
        using var fixture = new StoreFixture();
        await Register(fixture.Store, Guid.NewGuid(), "alice", "token-a");
        await new MobilePushNotificationService(fixture.Store, Enabled).EnqueueAsync("alice", "Update", "Details");
        await MobilePushDeliveryWorker.DeliverAsync(fixture.Store, new RecordingSender { Fail = true }, CancellationToken.None);
        var delivery = await fixture.Store.Deliveries.AsNoTracking().SingleAsync();
        Assert.Null(delivery.CompletedAt);
        Assert.Equal(1, delivery.Attempts);
        Assert.Equal("DeliveryFailed", delivery.LastError);
        Assert.True(delivery.NextAttemptAt > DateTime.UtcNow);
    }

    [Theory]
    [InlineData("https://malicious.example")]
    [InlineData("../users")]
    public async Task ArbitraryNotificationNavigationIsRejected(string section)
    {
        using var fixture = new StoreFixture();
        await Assert.ThrowsAsync<ArgumentException>(() => new MobilePushNotificationService(fixture.Store, Enabled).EnqueueAsync("alice", "Update", "Details", section));
        Assert.Empty(fixture.Store.Messages);
    }

    [Theory]
    [InlineData("", "Body", "home")]
    [InlineData("Title", " ", "home")]
    [InlineData("Title", "Body", "https://example.com")]
    public async Task PlaygroundRejectsInvalidMessages(string title, string body, string section)
    {
        using var fixture = new StoreFixture();
        var service = new MobilePushNotificationService(fixture.Store, Options.Create(new MobilePushOptions()));
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "alice") }, "test"));
        var result = await NotificationPlaygroundEndpoints.QueueMobilePush(new QueueMobilePushRequest(title, body, section), user, service, CancellationToken.None);
        Assert.Equal(400, Status(result));
        Assert.Empty(await fixture.Store.Messages.ToListAsync());
    }

    [Fact]
    public async Task PlaygroundUsesAuthenticatedRecipientAndTrimsContent()
    {
        using var fixture = new StoreFixture();
        var service = new MobilePushNotificationService(fixture.Store, Options.Create(new MobilePushOptions()));
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", "alice") }, "test"));
        var result = await NotificationPlaygroundEndpoints.QueueMobilePush(new QueueMobilePushRequest(" Title ", " Body ", "notifications"), user, service, CancellationToken.None);
        Assert.Equal(202, Status(result));
        var message = await fixture.Store.Messages.SingleAsync();
        Assert.Equal("alice", message.UserId);
        Assert.Equal("Title", message.Title);
        Assert.Equal("Body", message.Body);
    }

    [Fact]
    public async Task PlaygroundRequiresSubject()
    {
        using var fixture = new StoreFixture();
        var service = new MobilePushNotificationService(fixture.Store, Options.Create(new MobilePushOptions()));
        var result = await NotificationPlaygroundEndpoints.QueueMobilePush(new QueueMobilePushRequest("Title", "Body", "home"), new ClaimsPrincipal(), service, CancellationToken.None);
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
        Assert.Empty(await fixture.Store.Messages.ToListAsync());
    }

    private static int? Status(IResult result) => ((IStatusCodeHttpResult)result).StatusCode;

    private static Task<IResult> Register(MobilePushStore store, Guid id, string user, string token) => MobilePushEndpoints.Register(id, new MobilePushDeviceRegistration("Kotlin", "android", token), new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", user) }, "test")), store, Enabled, CancellationToken.None);

    private sealed class RecordingSender : IMobilePushSender
    {
        public bool Fail { get; set; }

        public List<string> Recipients { get; } = new();

        public Task SendAsync(MobilePushDevice device, MobilePushMessage message, CancellationToken cancellationToken)
        {
            if (Fail)
            {
                throw new InvalidOperationException("Sensitive provider response must not be stored.");
            }

            Recipients.Add(device.UserId);
            return Task.CompletedTask;
        }
    }

    private sealed class StoreFixture : IDisposable
    {
        private readonly SqliteConnection connection = new("Data Source=:memory:");
        private readonly NotificationTestContext context;

        public StoreFixture()
        {
            connection.Open();
            context = new NotificationTestContext(new DbContextOptionsBuilder<AdsDbContext>().UseSqlite(connection).Options);
            context.Database.EnsureCreated();
            Store = new MobilePushStore(context);
        }

        public MobilePushStore Store { get; }

        public void Dispose()
        {
            context.Dispose();
            connection.Dispose();
        }
    }

    // Isolate relational queue behavior from unrelated SQL Server-specific application tables.
    private sealed class NotificationTestContext : AdsDbContext
    {
        public NotificationTestContext(DbContextOptions<AdsDbContext> options) : base(options, NullLogger<AdsDbContext>.Instance)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var configuration = new MobilePushNotificationConfiguration();
            builder.ApplyConfiguration<MobilePushDevice>(configuration);
            builder.ApplyConfiguration<MobilePushMessage>(configuration);
            builder.ApplyConfiguration<MobilePushDelivery>(configuration);
        }
    }
}
