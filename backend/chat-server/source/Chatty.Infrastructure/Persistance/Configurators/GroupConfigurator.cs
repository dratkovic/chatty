using Chatty.Core.Infrastructure.Common.Persistence;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatty.Infrastructure.Persistance;

public class GroupConfigurator: AbstractAuditEntityConfiguration<Group>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Group> builder)
    {
        builder.Property( x => x.Name).IsRequired().HasMaxLength(64);

        builder.HasMany(x => x.Admins)
            .WithMany()
            .UsingEntity(x => x.ToTable("GroupAdmin"));
    }
}