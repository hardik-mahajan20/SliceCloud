using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IUnitService
{
    /// <summary>
    /// Retrieves a list of all units asynchronously.
    /// </summary>
    /// <returns>A task that returns a collection of unit view models.</returns>
    Task<List<UnitViewModel>> GetUnitsAsync();
}
