using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICountryRepository
{
    /// <summary>
    /// Retrieves all countries asynchronously.
    /// </summary>
    /// <returns>A task that returns the list of all countries.</returns>
    Task<List<Country>> GetAllCountriesAsync();
}
