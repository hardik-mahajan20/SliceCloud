using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICountryRepository
{
    /// <summary>
    /// Retrieves all countries as queryable.
    /// </summary>
    /// <returns>All countries as queryable.</returns>
    IQueryable<Country> GetAllCountriesAsQueryable();
}
