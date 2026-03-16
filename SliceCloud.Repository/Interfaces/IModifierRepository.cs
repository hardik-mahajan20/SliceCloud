using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierRepository
{
    /// <summary>
    /// Retrieves all modifiers as queryable.
    /// </summary>
    /// <returns>All modifiers as queryable.</returns>
    IQueryable<Modifier> GetAllModifiersAsQueryable();

    /// <summary>
    /// Adds a new modifier asynchronously in the database.
    /// </summary>
    /// <param name="modifier">The modifier entity to add.</param>
    /// <returns>A task that returns the ID of the created modifier.</returns>
    Task<int> AddModifierAsync(Modifier modifier);

    /// <summary>
    /// Updates an existing modifier asynchronously in the database.
    /// </summary>
    /// <param name="modifier">The modifier to update.</param>
    /// <returns>A task that returns the ID of the updated modifier.</returns>
    Task<int> UpdateModifierAsync(Modifier modifier);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
