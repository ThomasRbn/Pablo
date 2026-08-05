using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pablo.Infrastructure.Persistence;

public sealed class PabloDbContextFactory : IDesignTimeDbContextFactory<PabloDbContext>
{
    public PabloDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "../Pablo.API"));

        if (!Directory.Exists(basePath))
        {
            basePath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(), "src/Pablo.API"));
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' is missing or empty. Set ConnectionStrings:Default in appsettings or environment.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<PabloDbContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsql => npgsql.SetPostgresVersion(18, 0));

        return new PabloDbContext(optionsBuilder.Options);
    }
}