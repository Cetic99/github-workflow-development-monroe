using Microsoft.EntityFrameworkCore;
using CashVault.Domain.Common;

namespace CashVault.Infrastructure.PersistentStorage
{
    public class BaseRepository
    {
        protected readonly CashVaultContext _dbContext;

        public BaseRepository(CashVaultContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext), "Invalid value");
        }

        public async Task<T?> GetById<T>(int id) where T : Entity
        {
            return await _dbContext.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public void AddEntity<T>(T entity) where T : Entity
        {
            _dbContext.Add(entity);
        }

        public void UpdateEntity<T>(T entity) where T : Entity
        {
            _dbContext.Update(entity);
        }

        public void RemoveEntity<T>(T entity) where T : Entity
        {
            _dbContext.Remove(entity);
        }
    }
}
