namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations;

using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

internal class PostalServiceLocationConfiguration : IEntityTypeConfiguration<PostalServiceLocation>
{
    public void Configure(EntityTypeBuilder<PostalServiceLocation> builder)
    {
        builder.ToTable("PostalServiceLocation");

        builder.Property(o => o.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder
            .Property(e => e.Guid)
            .HasColumnType("CHAR(36)");

        var locationTypeConverter = new ValueConverter<PostalServiceLocationType, string>(
               v => v.Code,
               v => Enumeration.GetByCode<PostalServiceLocationType>(v)
           );

        builder
            .Property(e => e.LocationType)
            .HasConversion(locationTypeConverter)
            .HasColumnName("LocationType")
            .IsRequired();

        builder.OwnsOne(builder => builder.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.StreetName)
                .HasColumnName("StreetName")
                .IsRequired();

            addressBuilder.Property(a => a.StreetNumber)
                .HasColumnName("StreetNumber")
                .IsRequired();

            addressBuilder.Property(a => a.City)
                .HasColumnName("City")
                .IsRequired();

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .IsRequired();

            addressBuilder.Property(a => a.Country)
                .HasColumnName("Country")
                .IsRequired();
        });
    }
}

