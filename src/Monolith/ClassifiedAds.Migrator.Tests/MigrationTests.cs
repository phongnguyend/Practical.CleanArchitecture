using ClassifiedAds.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClassifiedAds.Migrator.Tests;

public class MigrationTests
{
    [Fact]
    public Task MigrationTests_GenerateExpectedScript()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AdsDbContext>();
        optionsBuilder.UseSqlServer("your-connection-string", sql =>
        {
            sql.MigrationsAssembly("ClassifiedAds.Migrator");
        });

        // Create a logger instance for the AdsDbContext
        var logger = NullLogger<AdsDbContext>.Instance;

        using var context = new AdsDbContext(optionsBuilder.Options, logger);

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