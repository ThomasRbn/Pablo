using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Pablo.API.Infrastructure.Identity;
using Pablo.API.Infrastructure.Persistence;

namespace Pablo.API.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Default' is missing or empty. Set ConnectionStrings:Default in appsettings or environment.");
        }

        services.AddDbContext<PabloDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.SetPostgresVersion(18, 0)));

        services.AddHealthChecks()
            .AddDbContextCheck<PabloDbContext>();

        // Identity stores only — no authentication scheme yet. Wire AddAuthentication
        // (e.g. JWT) and UseAuthentication/UseAuthorization when auth is implemented.
        services
            .AddIdentityCore<AuthenticationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<PabloDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}