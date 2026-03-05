using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUnitRepository
{
    /// <summary>
    /// Retrieves all units as queryable.
    /// </summary>
    /// <returns>All units as queryable.</returns>
    IQueryable<Unit> GetAllUnitsAsQueryable();
}
