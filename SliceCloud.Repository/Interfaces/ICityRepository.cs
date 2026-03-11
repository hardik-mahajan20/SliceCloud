using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICityRepository
{
    /// <summary>
    /// Retrieves all cities as queryable.
    /// </summary>
    /// <returns>All cities as queryable.</returns>
    IQueryable<City> GetAllCitiesAsQueryable();
}
