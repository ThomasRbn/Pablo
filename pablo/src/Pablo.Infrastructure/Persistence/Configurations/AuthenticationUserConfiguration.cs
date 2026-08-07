using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Pablo.Infrastructure.Identity;

namespace Pablo.Infrastructure.Persistence.Configurations;

public sealed class AuthenticationUserConfiguration : IEntityTypeConfiguration<AuthenticationUser>
{
    public void Configure(EntityTypeBuilder<AuthenticationUser> builder)
    {
        builder.ToTable("Users");
    }
}