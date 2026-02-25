using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICategoryRepository
{
    /// <summary>
    /// Retrieves all categories as quearyable.
    /// </summary>
    /// <returns>All categories as quearyable.</returns>
    IQueryable<Category> GetAllCategoriesAsQueryable();

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
