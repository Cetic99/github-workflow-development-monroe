using CashVault.Domain.Aggregates.TransactionAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.TransactionAggregate;
using CashVault.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transaction");

        builder.Property(o => o.Version)
        .IsRequired()
        .IsConcurrencyToken();

        builder.Property<int>("transactionTypeId")
        .UsePropertyAccessMode(PropertyAccessMode.Field)
        .HasColumnName("TransactionTypeId")
        .IsRequired();

        builder.HasOne(o => o.Type)
        .WithMany()
        .HasForeignKey("transactionTypeId");

        builder.Navigation(a => a.Type).AutoInclude();

        builder.Property<int>("transactionStatusId")
        .UsePropertyAccessMode(PropertyAccessMode.Field)
        .HasColumnName("StatusId")
        .IsRequired();

        builder.HasOne(o => o.Status)
        .WithMany()
        .HasForeignKey("transactionStatusId");

        builder.Navigation(a => a.Status).AutoInclude();

        builder.HasDiscriminator<string>("TransactionKind")
            .HasValue<TicketTransaction>(nameof(TicketTransaction))
            .HasValue<DispenserBillTransaction>(nameof(DispenserBillTransaction))
            .HasValue<AcceptorBillTransaction>(nameof(AcceptorBillTransaction))
            .HasValue<AcceptorCoinTransaction>(nameof(AcceptorCoinTransaction))
            .HasValue<ParcelLockerTransaction>(nameof(ParcelLockerTransaction));

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

internal class DispenserBillTransactionConfiguration : IEntityTypeConfiguration<DispenserBillTransaction>
{
    public void Configure(EntityTypeBuilder<DispenserBillTransaction> builder)
    {
        builder.OwnsMany(e => e.Items, a =>
        {
            a.ToTable("DispenserBillTransactionItem");
            a.WithOwner().HasForeignKey("TransactionId");
        });
    }
}

internal class TicketTransactionConfiguration : IEntityTypeConfiguration<TicketTransaction>
{
    public void Configure(EntityTypeBuilder<TicketTransaction> builder)
    {
        builder.HasOne(t => t.Ticket)
               .WithMany()
               .HasForeignKey(t => t.TicketId);

        builder.Property<int?>("ticketTypeId")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("TicketTypeId")
               .IsRequired();

        builder.Property<int?>("ticketTypeDetailId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("TicketTypeDetailId");

        builder.HasOne(o => o.TicketTypeDetail)
            .WithMany()
            .HasForeignKey("ticketTypeDetailId");

        builder.Navigation(a => a.TicketTypeDetail).AutoInclude();
    }
}


internal class ParcelLockerTransactionConfiguration : IEntityTypeConfiguration<ParcelLockerTransaction>
{
    public void Configure(EntityTypeBuilder<ParcelLockerTransaction> builder)
    {
        //builder.HasOne(t => t.Shipment)
        //       .WithMany()
        //       .HasForeignKey("ParcelLockerShipmentId");

        builder.Ignore(t => t.Shipment);
    }
}
