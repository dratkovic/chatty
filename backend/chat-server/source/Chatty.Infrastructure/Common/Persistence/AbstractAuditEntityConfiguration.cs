using Chatty.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatty.Infrastructure.Common.Persistence;

public abstract class AbstractAuditEntityConfiguration<T> :
    AbstractEntityConfiguration<T>, IEntityTypeConfiguration<T> where T : AuditEntityBase
{
    protected override void ConfigureEntity(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.ModifiedAt)
            .IsRequired();
        builder.Property(e => e.CreatedBy)
            .IsRequired();
        builder.Property(e => e.ModifiedBy)
            .IsRequired();
        
        base.Configure(builder);
    }
}

