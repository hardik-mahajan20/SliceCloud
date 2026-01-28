using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Interfaces;

public interface ICountryService
{
    /// <summary>
    /// Retrieves a list of all countries.
    /// </summary>
    /// <returns>An list of all countries.</returns>
    Task<List<Country>> GetAllCountriesAsync();
}
