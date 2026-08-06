using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pablo.Infrastructure.Persistence;

public sealed class PabloDbContextFactory : IDesignTimeDbContextFactory<PabloDbContext>
{
    public PabloDbContext CreateDbContext(string[] args)
    {
        var connectionFromArgs = TryGetConnectionArg(args);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(ResolveApiContentRoot())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = connectionFromArgs
            ?? configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' is missing or empty. Set ConnectionStrings:Default in appsettings or environment, or pass --connection.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<PabloDbContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsql => npgsql.SetPostgresVersion(18, 0));

        return new PabloDbContext(optionsBuilder.Options);
    }

    private static string? TryGetConnectionArg(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] is "--connection")
            {
                return args[i + 1];
            }
        }

        return null;
    }

    private static string ResolveApiContentRoot()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
             dir is not null;
             dir = dir.Parent)
        {
            foreach (var relative in new[]
                     {
                         "Pablo.API",
                         Path.Combine("src", "Pablo.API"),
                         Path.Combine("pablo", "src", "Pablo.API"),
                     })
            {
                var candidate = Path.Combine(dir.FullName, relative);
                if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException(
            "Could not locate Pablo.API for design-time configuration. Run from the repo or pass --connection.");
    }
}