using Microsoft.EntityFrameworkCore;

namespace Pablo.Infrastructure.Persistence;

public class PabloDbContext(DbContextOptions<PabloDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PabloDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}