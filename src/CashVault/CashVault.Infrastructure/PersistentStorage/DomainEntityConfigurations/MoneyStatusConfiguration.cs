using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations;

internal sealed class MoneyStatusConfiguration : IEntityTypeConfiguration<MoneyStatus>
{
    public void Configure(EntityTypeBuilder<MoneyStatus> builder)
    {
        builder.ToTable("MoneyStatus");

        builder.HasDiscriminator<string>("Key")
            .HasValue<DispenserBillCountStatus>("DispenserBillCountStatus")
            .HasValue<CurrentCreditStatus>("CurrentCreditStatus")
            .HasValue<BillTicketAcceptorStackerStatus>("BillTicketAcceptorStackerStatus")
            .HasValue<CoinAcceptorCollectorStatus>("CoinAcceptorCollectorStatus")
            .HasValue<ParcelLockerCurrentMoneyStatus>("ParcelLockerCurrentMoneyStatus");

        builder.Property(o => o.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(o => o.JsonValue)
            .HasColumnName("Value");

        builder.Ignore(o => o.DomainEvents);
    }
}

internal sealed class DispenserBillCountStatusConfiguration : IEntityTypeConfiguration<DispenserBillCountStatus>
{
    public void Configure(EntityTypeBuilder<DispenserBillCountStatus> builder)
    {
        builder.Ignore(x => x.Cassettes);
        builder.Ignore(x => x.RejectBin);
    }
}

internal sealed class CurrentCreditStatusConfiguration : IEntityTypeConfiguration<CurrentCreditStatus>
{
    public void Configure(EntityTypeBuilder<CurrentCreditStatus> builder)
    {
        builder.Ignore(x => x.Amount);

        builder.Ignore(x => x.Currency);
    }
}

internal sealed class BillTicketAcceptorStackerStatusConfiguration : IEntityTypeConfiguration<BillTicketAcceptorStackerStatus>
{
    public void Configure(EntityTypeBuilder<BillTicketAcceptorStackerStatus> builder)
    {
        builder.Ignore(x => x.BillCount);
        builder.Ignore(x => x.BillAmount);
        builder.Ignore(x => x.TicketCount);
        builder.Ignore(x => x.TicketAmount);
    }
}

internal sealed class CoinAcceptorCollectorStatusConfiguration : IEntityTypeConfiguration<CoinAcceptorCollectorStatus>
{
    public void Configure(EntityTypeBuilder<CoinAcceptorCollectorStatus> builder)
    {
        builder.Ignore(x => x.CoinCount);
        builder.Ignore(x => x.CoinAmount);
    }
}

internal sealed class ParcelLockerCurrentMoneyStatusConfiguration : IEntityTypeConfiguration<ParcelLockerCurrentMoneyStatus>
{
    public void Configure(EntityTypeBuilder<ParcelLockerCurrentMoneyStatus> builder)
    {
        builder.Ignore(x => x.BillCount);
        builder.Ignore(x => x.BillAmount);
        builder.Ignore(x => x.CoinCount);
        builder.Ignore(x => x.CoinAmount);
    }
}
