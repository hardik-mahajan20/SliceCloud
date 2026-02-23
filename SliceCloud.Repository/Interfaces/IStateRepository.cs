

using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IStateRepository
{
    /// <summary>
    /// Retrieves all states as queryable.
    /// </summary>
    /// <returns>List of states as queryable.</returns>
    IQueryable<State> GetAllStatesAsQueryable();
}
