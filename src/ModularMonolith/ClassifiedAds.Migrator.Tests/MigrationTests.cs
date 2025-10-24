using ClassifiedAds.Modules.AuditLog.Persistence;
using ClassifiedAds.Modules.Configuration.Persistence;
using ClassifiedAds.Modules.Identity.Persistence;
using ClassifiedAds.Modules.Notification.Persistence;
using ClassifiedAds.Modules.Product.Persistence;
using ClassifiedAds.Modules.Storage.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassifiedAds.Migrator.Tests;

public class MigrationTests
{
    [Fact]
    public Task MigrationTests_AuditLogDbContext_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditLogDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        using var context = new AuditLogDbContext(optionsBuilder.Options);

        var migrator = context.GetService<IMigrator>();

        // Act
        // Generate SQL script from initial migration to the latest
        string script = migrator.GenerateScript(
            fromMigration: null,  // null means from initial database
            toMigration: null,    // null means up to the latest migration
            options: MigrationsSqlGenerationOptions.Default);

        // Assert
        return Verify(script);
    }

    [Fact]
    public Task MigrationTests_ConfigurationDbContext_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        using var context = new ConfigurationDbContext(optionsBuilder.Options);

        var migrator = context.GetService<IMigrator>();

        // Act
        // Generate SQL script from initial migration to the latest
        string script = migrator.GenerateScript(
            fromMigration: null,  // null means from initial database
            toMigration: null,    // null means up to the latest migration
            options: MigrationsSqlGenerationOptions.Default);

        // Assert
        return Verify(script);
    }

    [Fact]
    public Task MigrationTests_IdentityDbContext_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        using var context = new IdentityDbContext(optionsBuilder.Options);

        var migrator = context.GetService<IMigrator>();

        // Act
        // Generate SQL script from initial migration to the latest
        string script = migrator.GenerateScript(
            fromMigration: null,  // null means from initial database
            toMigration: null,    // null means up to the latest migration
            options: MigrationsSqlGenerationOptions.Default);

        // Assert
        return Verify(script);
    }

    [Fact]
    public Task MigrationTests_NotificationDbContext_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        using var context = new NotificationDbContext(optionsBuilder.Options);

        var migrator = context.GetService<IMigrator>();

        // Act
        // Generate SQL script from initial migration to the latest
        string script = migrator.GenerateScript(
            fromMigration: null,  // null means from initial database
            toMigration: null,    // null means up to the latest migration
            options: MigrationsSqlGenerationOptions.Default);

        // Assert
        return Verify(script);
    }

    [Fact]
    public Task MigrationTests_ProductDbContext_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        using var context = new ProductDbContext(optionsBuilder.Options);

        var migrator = context.GetService<IMigrator>();

        // Act
        // Generate SQL script from initial migration to the latest
        string script = migrator.GenerateScript(
            fromMigration: null,  // null means from initial database
            toMigration: null,    // null means up to the latest migration
            options: MigrationsSqlGenerationOptions.Default);

        // Assert
        return Verify(script);
    }

    [Fact]
    public Task MigrationTests_StorageDbContext_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<StorageDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        using var context = new StorageDbContext(optionsBuilder.Options);

        var migrator = context.GetService<IMigrator>();

        // Act
        // Generate SQL script from initial migration to the latest
        string script = migrator.GenerateScript(
            fromMigration: null,  // null means from initial database
            toMigration: null,    // null means up to the latest migration
            options: MigrationsSqlGenerationOptions.Default);

        // Assert
        return Verify(script);
    }
}