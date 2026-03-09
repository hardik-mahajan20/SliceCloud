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
    /// Adds a list of ModifierGroupModifierMapping asynchronously.
    /// </summary>
    /// <param name="modifierGroupModifierMappings">The list of ModifierGroupModifierMapping to add.</param>
    Task AddModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings);

    /// <summary>
    /// Removes a list of ModifierGroupModifierMapping asynchronously.
    /// </summary>
    /// <param name="modifierGroupModifierMappings">The list of ModifierGroupModifierMapping to add.</param>
    Task RemoveModifierGroupMappingsAsync(List<ModifierGroupModifierMapping> modifierGroupModifierMappings);
}
