using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations
{
    internal class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Ticket");

            builder.Property(o => o.Version)
                .IsRequired()
                .IsConcurrencyToken();

            builder.Property<int>("ticketTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("TicketTypeId")
                .IsRequired();

            builder.HasOne(o => o.Type)
                .WithMany()
                .HasForeignKey("ticketTypeId");

            builder.Navigation(a => a.Type).AutoInclude();

            builder
                .Property(e => e.Guid)
                .HasColumnType("CHAR(36)");

            var currencyConverter = new ValueConverter<Currency, string>(
                v => v.Code,
                v => Enumeration.GetByCode<Currency>(v)
            );

            builder
                .Property(e => e.Currency)
                .HasConversion(currencyConverter)
                .HasColumnName("Currency")
                .IsRequired();
        }
    }
}
