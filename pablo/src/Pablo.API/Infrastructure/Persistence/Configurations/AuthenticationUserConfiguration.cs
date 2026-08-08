using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Pablo.API.Infrastructure.Identity;

namespace Pablo.API.Infrastructure.Persistence.Configurations;

public sealed class AuthenticationUserConfiguration : IEntityTypeConfiguration<AuthenticationUser>
{
    public void Configure(EntityTypeBuilder<AuthenticationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.DisplayName)
            .HasMaxLength(256)
            .IsRequired();
    }
}