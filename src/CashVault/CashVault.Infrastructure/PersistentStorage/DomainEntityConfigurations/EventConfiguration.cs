using CashVault.Domain.Common.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations;

internal class EventConfiguration : IEntityTypeConfiguration<BaseEvent>
{
    public void Configure(EntityTypeBuilder<BaseEvent> builder)
    {
        builder.ToTable("EventLog");

        builder.HasKey(e => e.Id);

        builder
        .Property(e => e.Guid)
        .HasColumnType("CHAR(36)");

        builder.Property(e => e.Created);

        builder.Property(e => e.Message);

        builder.Property(e => e.EventName);

        builder.HasDiscriminator<string>("EventType")
                .HasValue<TransactionEvent>(nameof(TransactionEvent))
                .HasValue<DeviceFailEvent>(nameof(DeviceFailEvent))
                .HasValue<DeviceEvent>(nameof(DeviceEvent))
                .HasValue<DeviceWarningEvent>(nameof(DeviceWarningEvent));

        //ignored properties
        builder.Ignore(e => e.Type);
    }
}

internal class DeviceEventConfiguration : IEntityTypeConfiguration<DeviceEvent>
{
    public void Configure(EntityTypeBuilder<DeviceEvent> builder)
    {
        builder.HasBaseType<BaseEvent>();

        builder.Property(e => e.DeviceType);
    }
}


internal class DeviceFailEventConfiguration : IEntityTypeConfiguration<DeviceFailEvent>
{
    public void Configure(EntityTypeBuilder<DeviceFailEvent> builder)
    {
        builder.HasBaseType<DeviceEvent>();
    }
}


internal class DeviceWarningEventConfiguration : IEntityTypeConfiguration<DeviceWarningEvent>
{
    public void Configure(EntityTypeBuilder<DeviceWarningEvent> builder)
    {
        builder.HasBaseType<DeviceEvent>();
    }
}