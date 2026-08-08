using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Pablo.API.Infrastructure.Persistence;

namespace Pablo.API.Tests;

public class PabloApiFactory : WebApplicationFactory<Program>
{
    // Unique per factory instance so parallel test classes do not share InMemory state.
    private readonly string _databaseName = Guid.NewGuid().ToString("N");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        // UseSetting is applied before Program reads configuration (CreateBuilder).
        builder.UseSetting(
            "ConnectionStrings:Default",
            "Host=localhost;Database=pablo-tests;Username=test;Password=test");
        builder.UseSetting("Jwt:Issuer", "pablo-api-tests");
        builder.UseSetting("Jwt:Audience", "pablo-web-tests");
        builder.UseSetting("Jwt:SigningKey", "test-only-signing-key-at-least-32b");
        builder.UseSetting("Jwt:ExpirationMinutes", "60");

        builder.ConfigureTestServices(services =>
        {
            // EF Core 8+ keeps provider config via IDbContextOptionsConfiguration;
            // remove those or UseNpgsql still wins over a later UseInMemoryDatabase.
            services.RemoveAll<IDbContextOptionsConfiguration<PabloDbContext>>();
            services.RemoveAll<DbContextOptions<PabloDbContext>>();
            services.RemoveAll<PabloDbContext>();

            services.AddDbContext<PabloDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}