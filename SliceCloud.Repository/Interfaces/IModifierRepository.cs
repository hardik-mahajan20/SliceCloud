using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierRepository
{
    /// <summary>
    /// Retrieves all modifier-group-modifier-mapping as queryable.
    /// </summary>
    /// <returns>All modifier-group-modifier-mapping as queryable.</returns>
    IQueryable<ModifierGroupModifierMapping> GetModifierGroupModifierMappingAsQueryable();
}
