

using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Interfaces;

public interface IStateService
{
    /// <summary>
    /// Retrieves a states by its country id asynchronously.
    /// </summary>
    /// <param name="countryId">The ID of the country to retrieve states for.</param>
    /// <returns>A task that returns a list of state.</returns>
    Task<List<State>> GetStatesByCountryIdAsync(int countryId);
}
