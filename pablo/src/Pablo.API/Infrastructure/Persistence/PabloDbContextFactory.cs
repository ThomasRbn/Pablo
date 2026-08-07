using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.API.Infrastructure.Persistence;

public sealed class PabloDbContextFactory : IDesignTimeDbContextFactory<PabloDbContext>
{
    public PabloDbContext CreateDbContext(string[] args)
    {
        var connectionString = TryGetConnectionArg(args);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(ResolveContentRoot())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            connectionString = configuration.GetConnectionString("Default");
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' is missing or empty. Set ConnectionStrings:Default in appsettings or environment, or pass --connection.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<PabloDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PabloDbContext(optionsBuilder.Options);
    }

    private static string? TryGetConnectionArg(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] is not "--connection")
            {
                continue;
            }

            return i + 1 < args.Length ? args[i + 1] : null;
        }

        return null;
    }

    private static string ResolveContentRoot()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
             dir is not null;
             dir = dir.Parent)
        {
            foreach (var relative in new[]
                     {
                         ".",
                         "Pablo.API",
                         Path.Combine("src", "Pablo.API"),
                         Path.Combine("pablo", "src", "Pablo.API"),
                     })
            {
                var candidate = Path.GetFullPath(Path.Combine(dir.FullName, relative));
                if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException(
            "Could not locate Pablo.API appsettings.json for design-time configuration. Run from the repo or pass --connection.");
    }
}