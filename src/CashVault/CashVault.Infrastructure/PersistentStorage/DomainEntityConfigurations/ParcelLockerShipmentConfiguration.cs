using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using CashVault.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations;

internal class ParcelLockerShipmentConfiguration : IEntityTypeConfiguration<ParcelLockerShipment>
{
    public void Configure(EntityTypeBuilder<ParcelLockerShipment> builder)
    {
        builder.ToTable("ParcelLockerShipment");

        builder.Property(o => o.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder
            .Property(e => e.Guid)
            .HasColumnType("CHAR(36)");

        builder.Property<int>("shipmentStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ShipmentStatusId")
                .IsRequired();

        builder.HasOne(o => o.Status)
               .WithMany()
               .HasForeignKey("shipmentStatusId");

        builder.Navigation(a => a.Status).AutoInclude();

        var currencyConverter = new ValueConverter<Currency?, string?>(
            v => v == null ? string.Empty : v.Code,
            v => string.IsNullOrEmpty(v) ? null : Enumeration.GetByCode<Currency>(v)
        );

        builder
            .Property(e => e.Currency)
            .HasConversion(currencyConverter)
            .HasColumnName("Currency")
            .IsRequired(false);

        var locationTypeConverter = new ValueConverter<PostalServiceLocationType?, string?>(
              v => v == null ? string.Empty : v.Code,
              v => string.IsNullOrEmpty(v) ? null : Enumeration.GetByCode<PostalServiceLocationType>(v)
          );

        builder
            .Property(e => e.AddressLocationType)
            .HasConversion(locationTypeConverter)
            .HasColumnName("LocationType")
            .IsRequired(false);

        builder.OwnsOne(builder => builder.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.StreetName)
                .HasColumnName("StreetName")
                .IsRequired(false);

            addressBuilder.Property(a => a.StreetNumber)
                .HasColumnName("StreetNumber")
                .IsRequired(false);

            addressBuilder.Property(a => a.City)
                .HasColumnName("City")
                .IsRequired(false);

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .IsRequired(false);

            addressBuilder.Property(a => a.Country)
                .HasColumnName("Country")
                .IsRequired(false);
        });

        builder.OwnsOne(builder => builder.Sender, senderBuilder =>
        {
            senderBuilder.Property(a => a.FirstName)
                .HasColumnName("SenderFirstName")
                .IsRequired(false);
            senderBuilder.Property(a => a.LastName)
                .HasColumnName("SenderLastName")
                .IsRequired(false);
            senderBuilder.Property(a => a.PhoneNumber)
                .HasColumnName("SenderPhoneNumber")
                .IsRequired(false);
        });

        builder.OwnsOne(builder => builder.Reciever, recieverBuilder =>
        {
            recieverBuilder.Property(a => a.FirstName)
                .HasColumnName("RecieverFirstName")
                .IsRequired(false);
            recieverBuilder.Property(a => a.LastName)
                .HasColumnName("RecieverLastName")
                .IsRequired(false);
            recieverBuilder.Property(a => a.PhoneNumber)
                .HasColumnName("RecieverPhoneNumber")
                .IsRequired(false);
        });

        builder.Ignore(o => o.DomainEvents);
    }
}

internal class ParcelLockerShipmentStatusConfiguration : IEntityTypeConfiguration<ShipmentStatus>
{
    public void Configure(EntityTypeBuilder<ShipmentStatus> builder)
    {
        builder.ToTable("ParcelLockerShipmentStatus");

    }
}
