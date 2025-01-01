using Chatty.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatty.Infrastructure.Persistance.User;
public class UserConfigurations : AbstractAuditEntityConfiguration<Domain.User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Domain.User> builder)
    {
        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property("_passwordHash")
            .HasColumnName("PasswordHash")
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
    }
}