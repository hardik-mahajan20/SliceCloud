using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierGroupRepository
{
    /// <summary>
    /// Retrieves all modifier-group as queryable.
    /// </summary>
    /// <returns>All modifier-group as queryable.</returns>
    IQueryable<ModifierGroup> GetAllModifierGroupsAsQueryable();

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
