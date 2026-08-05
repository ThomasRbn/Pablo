using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Pablo.Infrastructure.Identity;

namespace Pablo.Infrastructure.Persistence;

public class PabloDbContext(DbContextOptions<PabloDbContext> options)
    : IdentityDbContext<AuthenticationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PabloDbContext).Assembly);
    }
}