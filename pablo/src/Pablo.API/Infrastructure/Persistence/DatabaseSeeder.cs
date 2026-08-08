using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Pablo.API.Infrastructure.Identity;

namespace Pablo.API.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public const string RootUsername = "root";
    // Meets Identity's default complexity rules (dev-only seed; never used in production).
    public const string RootPassword = "Root1!dev";
    public const string RootEmail = "root@localhost";
    public const string RootDisplayName = "Administrator";

    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthenticationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(DatabaseSeeder).FullName!);

        if (await userManager.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var user = new AuthenticationUser
        {
            UserName = RootUsername,
            Email = RootEmail,
            EmailConfirmed = true,
            DisplayName = RootDisplayName,
        };

        var result = await userManager.CreateAsync(user, RootPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to seed root user: {errors}");
        }

        logger.LogInformation(
            "Seeded initial user '{Username}' ({Email})",
            RootUsername,
            RootEmail);
    }
}