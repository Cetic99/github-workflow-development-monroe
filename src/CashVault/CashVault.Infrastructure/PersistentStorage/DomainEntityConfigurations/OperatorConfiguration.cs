using CashVault.Domain.Aggregates.OperatorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations
{
    internal class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        public void Configure(EntityTypeBuilder<Operator> builder)
        {
            builder.ToTable("Operator");

            builder.Property(o => o.Version)
                .IsRequired()
                .IsConcurrencyToken();

            builder
                .Property(e => e.Guid)
                .HasColumnType("CHAR(36)");

            builder
                .HasMany(o => o.OperatorPermissions)
                .WithOne(x => x.Operator);

            builder.Ignore(o => o.DomainEvents);
            builder.Ignore(o => o.Permissions);
        }
    }

    internal class IdentificationCardConfiguration : IEntityTypeConfiguration<IdentificationCard>
    {
        public void Configure(EntityTypeBuilder<IdentificationCard> builder)
        {
            builder.ToTable("IdentificationCard");

            builder.Property(o => o.Version)
                .IsRequired()
                .IsConcurrencyToken();

            builder
                .Property(e => e.Guid)
                .HasColumnType("CHAR(36)");

            builder.Property<int>("identificationCardStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("IdentificationCardStatusId")
                .IsRequired();

            builder.HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey("identificationCardStatusId");

            builder.Navigation(a => a.Status).AutoInclude();
        }
    }

    internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permission");
        }
    }

    internal class OperatorPermissionConfiguration : IEntityTypeConfiguration<OperatorPermission>
    {
        public void Configure(EntityTypeBuilder<OperatorPermission> builder)
        {
            builder.ToTable("OperatorPermission");

            builder.Property(o => o.Version)
                .IsRequired()
                .IsConcurrencyToken();

            builder
                .Property(e => e.Guid)
                .HasColumnType("CHAR(36)");

            builder
                .Property<int>("permissionId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PermissionId")
                .IsRequired();

            builder
                .HasOne(o => o.Permission)
                .WithMany()
                .HasForeignKey("permissionId");

            builder.Navigation(a => a.Permission).AutoInclude();

            builder.HasOne(x => x.Operator);
        }
    }

    internal class IdentificationCardStatusConfiguration : IEntityTypeConfiguration<IdentificationCardStatus>
    {
        public void Configure(EntityTypeBuilder<IdentificationCardStatus> builder)
        {
            builder.ToTable("IdentificationCardStatus");

        }
    }
}
