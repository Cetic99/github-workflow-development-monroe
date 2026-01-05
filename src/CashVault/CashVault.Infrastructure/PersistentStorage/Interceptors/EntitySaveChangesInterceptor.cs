using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashVault.Domain;
using CashVault.Domain.Common;
using CashVault.Application.Interfaces;

namespace CashVault.Infrastructure.PersistentStorage.Interceptors
{
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ISessionService _sessionService;

        public EntitySaveChangesInterceptor(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            string currentUser = _sessionService.User.Username;
            DateTime currentDateTime = DateTime.UtcNow;

            if (context == null) return;

            var entries = context.ChangeTracker.Entries<Entity>();

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    var createdByProperty = typeof(Entity)
                        .GetProperty("CreatedBy");

                    if (createdByProperty != null)
                    {
                        createdByProperty.SetValue(entity, currentUser);
                    }

                    var createdProperty = typeof(Entity)
                            .GetProperty("Created");

                    if (createdProperty != null)
                    {
                        createdProperty.SetValue(entity, currentDateTime);
                    }

                    var versionProperty = typeof(Entity)
                        .GetProperty("Version");

                    if (versionProperty != null)
                    {
                        versionProperty.SetValue(entity, 1);
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    var updatedByProperty = typeof(Entity)
                        .GetProperty("UpdatedBy");

                    if (updatedByProperty != null)
                    {
                        updatedByProperty.SetValue(entity, currentUser);
                    }

                    var updatedProperty = typeof(Entity)
                            .GetProperty("Updated");

                    if (updatedProperty != null)
                    {
                        updatedProperty.SetValue(entity, currentDateTime);
                    }

                    var versionProperty = typeof(Entity)
                        .GetProperty("Version");
                    
                    if (versionProperty != null)
                    {
                        var currentVersion = (int)versionProperty.GetValue(entity);
                        versionProperty.SetValue(entity, currentVersion + 1);
                    }
                }
                    
            }
        }
    }
}
