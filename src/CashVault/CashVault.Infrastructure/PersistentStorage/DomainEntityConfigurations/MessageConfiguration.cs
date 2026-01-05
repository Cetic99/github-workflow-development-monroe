using CashVault.Domain.Aggregates.MessageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations
{
    internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");

            builder.Property(o => o.Version)
                .IsRequired()
                .IsConcurrencyToken();

            builder
                .Property(e => e.Guid)
                .HasColumnType("CHAR(36)");
        }
    }
}
