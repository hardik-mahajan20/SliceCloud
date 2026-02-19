using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICityRepository
{
    /// <summary>
    /// Retrieves all cities as quearyable.
    /// </summary>
    /// <returns>All cities as quearyable.</returns>
    IQueryable<City> GetAllCitiesAsQuearyable();
}
