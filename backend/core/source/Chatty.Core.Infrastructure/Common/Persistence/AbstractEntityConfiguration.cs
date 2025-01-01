using Chatty.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatty.Core.Infrastructure.Common.Persistence;

public abstract class AbstractEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : AuditEntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.ModifiedAt)
            .IsRequired();
        builder.Property(e => e.CreatedBy)
            .IsRequired();
        builder.Property(e => e.ModifiedBy)
            .IsRequired();

        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}

