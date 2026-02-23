using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICountryRepository
{
    /// <summary>
    /// Retrieves all countries as quearyable.
    /// </summary>
    /// <returns>All countries as quearyable.</returns>
    IQueryable<Country> GetAllCountruiesAsQuearyable();
}
