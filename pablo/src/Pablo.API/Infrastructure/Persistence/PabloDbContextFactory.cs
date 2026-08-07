using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.API.Infrastructure.Persistence;

public sealed class PabloDbContextFactory : IDesignTimeDbContextFactory<PabloDbContext>
{
    public PabloDbContext CreateDbContext(string[] args)
    {
        var connectionFromArgs = TryGetConnectionArg(args);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(ResolveContentRoot())
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

    private static string ResolveContentRoot()
    {
        for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
             dir is not null;
             dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "appsettings.json")))
            {
                return dir.FullName;
            }
        }

        throw new InvalidOperationException(
            "Could not locate appsettings.json for design-time configuration. Run from the Pablo.API project directory or pass --connection.");
    }
}