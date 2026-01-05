using System.Text.Json;
using CashVault.Domain.Aggregates.DeviceAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations;

internal sealed class MoneyStatusTransactionConfiguration : IEntityTypeConfiguration<MoneyStatusTransaction>
{
    public void Configure(EntityTypeBuilder<MoneyStatusTransaction> builder)
    {
        builder.ToTable("MoneyStatusTransaction");

        builder.Property(o => o.Version)
        .IsRequired()
        .IsConcurrencyToken();

        builder.Property<int>("moneyStatusTransactionTypeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("MoneyStatusTransactionTypeId")
            .IsRequired();

        builder.HasOne(o => o.Type)
            .WithMany()
            .HasForeignKey("moneyStatusTransactionTypeId");

        builder.Navigation(a => a.Type).AutoInclude();

        builder
                .Property(e => e.Guid)
                .HasColumnType("CHAR(36)");

        var deviceTypeConverter = new ValueConverter<DeviceType, string>(
                v => v.Code,
                v => Enumeration.GetByCode<DeviceType>(v)
            );

        builder
            .Property(e => e.DeviceType)
            .HasConversion(deviceTypeConverter)
            .HasColumnName("DeviceType")
            .IsRequired();

        builder.Property<int>("transactionStatusId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("StatusId")
            .IsRequired();

        builder.HasOne(o => o.Status)
            .WithMany()
            .HasForeignKey("transactionStatusId");

        builder.Navigation(a => a.Status).AutoInclude();

        var currencyConverter = new ValueConverter<Currency, string>(
            v => v.Code,
            v => Enumeration.GetByCode<Currency>(v)
        );

        builder
            .Property(e => e.Currency)
            .HasConversion(currencyConverter)
            .HasColumnName("Currency")
            .IsRequired();

        builder
            .Property(e => e.OldDispenserBillCountStatus)
            .HasConversion(new JsonValueConverter<DispenserBillCountStatus>())
            .HasColumnName("OldDispenserBillCountStatus")
            .IsRequired();

        builder
            .Property(e => e.NewDispenserBillCountStatus)
            .HasConversion(new JsonValueConverter<DispenserBillCountStatus>())
            .HasColumnName("NewDispenserBillCountStatus")
            .IsRequired();

        builder
            .Property(e => e.OldBillTicketAcceptorStackerStatus)
            .HasConversion(new JsonValueConverter<BillTicketAcceptorStackerStatus>())
            .HasColumnName("OldAcceptorBillCountStatus")
            .IsRequired();

        builder
            .Property(e => e.NewBillTicketAcceptorStackerStatus)
            .HasConversion(new JsonValueConverter<BillTicketAcceptorStackerStatus>())
            .HasColumnName("NewAcceptorBillCountStatus")
            .IsRequired();
    }
}


internal class JsonValueConverter<T> : ValueConverter<T, string>
{
    public JsonValueConverter()
        : base(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }),
            v => string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize<T>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        )
    { }
}
