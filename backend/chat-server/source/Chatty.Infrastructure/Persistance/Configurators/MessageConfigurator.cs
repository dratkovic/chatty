using Chatty.Core.Infrastructure.Common.Persistence;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatty.Infrastructure.Persistance;

public class MessageConfigurator: AbstractEntityConfiguration<Message>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Message> builder)
    {
        builder.Property( x => x.Content).IsRequired().HasMaxLength(1024);

        builder.HasIndex(x => x.TimeStampUtc).IsDescending();
    }
}