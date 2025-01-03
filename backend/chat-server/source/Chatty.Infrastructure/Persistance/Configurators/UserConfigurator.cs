using Chatty.Core.Infrastructure.Common.Persistence;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatty.Infrastructure.Persistance;

public class UserConfigurator: AbstractAuditEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.Property( x => x.Email).IsRequired().HasMaxLength(128);
        builder.Property( x => x.DisplayName).IsRequired().HasMaxLength(64);

        builder.HasMany(x => x.Groups)
            .WithMany(x => x.Participants)
            .UsingEntity(x => x.ToTable("UserGroups"));
        
        builder.HasIndex(x => x.Email).IsUnique();
    }
}