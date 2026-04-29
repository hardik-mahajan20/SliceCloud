using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierGroupModifierMappingsRepository
{
    /// <summary>
    /// Retrieves all modifierGroupModifierMapping as queryable.
    /// </summary>
    /// <returns>All modifierGroupModifierMapping as queryable.</returns>
    IQueryable<ModifierGroupModifierMapping> GetAllModifierGroupModifierMappingAsQueryable();

    /// <summary>
    /// Adds a new list of modifierGroupModifierMapping asynchronously in the database.
    /// </summary>
    /// <param name="modifierGroupModifierMappings">The list of modifierGroupModifierMapping entity to add.</param>
    /// <returns>A task that returns the ID of the created modifierGroupModifierMapping.</returns>
    Task<int> AddModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings);

    /// <summary>
    /// Removes a list of modifierGroupModifierMapping asynchronously.
    /// </summary>
    /// <param name="modifierGroupModifierMappings">The list of modifierGroupModifierMapping to remove.</param>
    Task<bool> RemoveModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings);
}
