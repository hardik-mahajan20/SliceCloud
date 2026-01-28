

using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IStateRepository
{
    /// <summary>
    /// Retrieves all states by its country id asynchronously.
    /// </summary>
    /// <param name="id">The ID of the country to retrieve states for.</param>
    /// <returns>A task that returns the list of states.</returns>
    Task<List<State>> GetStatesByCountryIdAsync(int countryId);
}
