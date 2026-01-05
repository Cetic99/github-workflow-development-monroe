using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashVault.Infrastructure.PersistentStorage.DomainEntityConfigurations
{
    internal sealed class ConfigConfiguration : IEntityTypeConfiguration<Configuration>
    {
        public void Configure(EntityTypeBuilder<Configuration> builder)
        {
            builder.ToTable("Configuration");
        }
    }
}
