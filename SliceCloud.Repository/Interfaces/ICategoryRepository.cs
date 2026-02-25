using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICategoryRepository
{
    /// <summary>
    /// Retrieves all categories as quearyable.
    /// </summary>
    /// <returns>All categories as quearyable.</returns>
    IQueryable<Category> GetAllCategoriesAsQueryable();
}
