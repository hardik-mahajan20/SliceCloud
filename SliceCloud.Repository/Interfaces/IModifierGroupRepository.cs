using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierGroupRepository
{
    /// <summary>
    /// Retrieves all modifierGroup as queryable.
    /// </summary>
    /// <returns>All modifierGroup as queryable.</returns>
    IQueryable<ModifierGroup> GetAllModifierGroupsAsQueryable();

    /// <summary>
    /// Retrieves a modifierGroup by its ID asynchronously.
    /// </summary>
    /// <param name="modifierGroupId">The ID of the modifierGroup to retrieve.</param>
    /// <returns>A task that returns the modifierGroup if found in the database, otherwise null.</returns>
    Task<ModifierGroup?> GetModifierGroupByIdAsync(int modifierGroupId);

    /// <summary>
    /// Adds a new modifierGroup asynchronously in the database.
    /// </summary>
    /// <param name="modifierGroup">The modifierGroup entity to add.</param>
    /// <returns>A task that returns the ID of the created modifierGroup.</returns>
    Task<int> AddModifierGroupAsync(ModifierGroup modifierGroup);

    /// <summary>
    /// Updates an existing modifierGroup asynchronously in the database.
    /// </summary>
    /// <param name="modifierGroup">The modifierGroup to update.</param>
    /// <returns>A task that returns the ID of the updated modifierGroup.</returns>
    Task<int> UpdateModifierGroupAsync(ModifierGroup modifierGroup);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
