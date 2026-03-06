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

    /// <summary>
    /// Adds a new modifier group asynchronously.
    /// </summary>
    /// <param name="modifierGroup">The modifier group to add.</param>
    /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
    Task<int> AddModifierGroupAsync(ModifierGroup modifierGroup);
}
