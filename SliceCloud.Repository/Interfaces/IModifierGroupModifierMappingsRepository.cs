using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierGroupModifierMappingsRepository
{
    /// <summary>
    /// Retrieves all modifierGroupModifierMapping as queryable.
    /// </summary>
    /// <returns>All modifierGroupModifierMapping as queryable.</returns>
    IQueryable<ModifierGroupModifierMapping> GetAllModifierGroupModifierMappingAsQueryable();
}
