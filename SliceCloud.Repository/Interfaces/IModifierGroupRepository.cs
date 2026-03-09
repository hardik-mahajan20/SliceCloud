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

    /// <summary>
    /// Retrieves a modifierGroup by its ID asynchronously.
    /// </summary>
    /// <param name="modifierGroupId">The ID of the modifierGroup to retrieve.</param>
    /// <returns>A task that returns the modifierGroup if found in the database, otherwise null.</returns>
    Task<ModifierGroup?> GetModifierGroupByIdAsync(int modifierGroupId);

    /// <summary>
    /// Updates an existing modifierGroup asynchronously in the database.
    /// </summary>
    /// <param name="modifierGroup">The modifierGroup to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateModifierGroupAsync(ModifierGroup modifierGroup);
}
