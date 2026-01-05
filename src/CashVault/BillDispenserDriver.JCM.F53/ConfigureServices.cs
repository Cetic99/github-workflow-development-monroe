using CashVault.Domain.Aggregates.DeviceAggregate.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CashVault.BillDispenserDriver.JCM.F53
{
    public class ConfigureServices
    {
        public static void AddServices(IServiceCollection services)
        {
            // Add services here
            services.AddSingleton<IBillDispenser, BillDispenserDriver>();
        }
    }
}
