using CashVault.Domain.Aggregates.MessageAggregate;
using CashVault.Domain.Aggregates.MoneyStatusAggregate;
using CashVault.Domain.Aggregates.OperatorAggregate;
using CashVault.Domain.Aggregates.ParcelLockerAggregate;
using CashVault.Domain.Aggregates.TicketAggregate;
using CashVault.Domain.Common.Events;
using CashVault.Domain.TransactionAggregate;
using CashVault.Infrastructure.PersistentStorage.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CashVault.Infrastructure.PersistentStorage;

public class CashVaultContext : DbContext
{
    #region Entities
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<MoneyStatus> MoneyStatuses { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<OperatorPermission> OperatorPermissions { get; set; }
    public DbSet<IdentificationCard> IdentificationCards { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<BaseEvent> EventLogs { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MoneyStatusTransaction> MoneyStatusTransactions { get; set; }
    public DbSet<ParcelLockerShipment> ParcelLockerShipments { get; set; }
    public DbSet<PostalServiceLocation> PostalServiceLocations { get; set; }

    #endregion

    private readonly EntitySaveChangesInterceptor _entitySaveChangesInterceptor;
    private readonly string _connectionString;

    public CashVaultContext(DbContextOptions<CashVaultContext> options, EntitySaveChangesInterceptor entitySaveChangesInterceptor, IConfiguration configuration)
        : base(options)
    {
        _entitySaveChangesInterceptor = entitySaveChangesInterceptor;
        _connectionString = configuration.GetConnectionString("CashVaultDatabase");

        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new ArgumentNullException("Connection string is not set.");
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_entitySaveChangesInterceptor);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseFirebird(_connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CashVaultContext).Assembly);
    }
}
