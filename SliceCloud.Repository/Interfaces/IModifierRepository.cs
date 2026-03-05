using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IModifierRepository
{
    /// <summary>
    /// Retrieves all modifiers as queryable.
    /// </summary>
    /// <returns>All modifiers as queryable.</returns>
    IQueryable<Modifier> GetAllModifiersAsQueryable();
}
