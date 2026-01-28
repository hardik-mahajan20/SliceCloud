using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICityRepository
{
    /// <summary>
    /// Retrieves all cities by its state id asynchronously.
    /// </summary>
    /// <param name="id">The ID of the state to retrieve cities for.</param>
    /// <returns>A task that returns the list of cities.</returns>
    Task<List<City>> GetCitiesByStateIdAsync(int stateId);
}
