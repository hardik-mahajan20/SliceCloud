using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierGroupRepository
{
    /// <summary>
    /// Retrieves all modifier-group as queryable.
    /// </summary>
    /// <returns>All modifier-group as queryable.</returns>
    IQueryable<ModifierGroup> GetAllModifierGroupsAsQueryable();
}
