using CashVault.Domain.Common;
using CashVault.Domain;

namespace CashVault.Application.Interfaces.Persistence
{
    /// <summary>
    /// A base repository for handling database commands.
    /// </summary>
    public interface IBaseRepository<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Gets an entity by its unique identifier.
        /// </summary>
        /// <typeparam name="S">The type of the entity.</typeparam>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity with the specified unique identifier.</returns>
        public Task<S?> GetById<S>(int id) where S : Entity;

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <typeparam name="S">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be added.</param>
        public void AddEntity<S>(S entity) where S : Entity;

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <typeparam name="S">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be updated.</param>
        public void UpdateEntity<S>(S entity) where S : Entity;

        public void RemoveEntity<S>(S entity) where S : Entity;
    }
}
