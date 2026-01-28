using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Interfaces;

public interface ICityService
{
    /// <summary>
    /// Retrieves a list of all cities by state.
    /// </summary>
    /// <param name="stateId">The stateId of the for which cities should be retrieve.</param>
    /// <returns>An list of all cities by its state.</returns>
    Task<List<City>> GetCitiesByStateIdAsync(int stateId);
}
