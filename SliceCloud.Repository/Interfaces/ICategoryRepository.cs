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
    /// Adds a new category asynchronously to the database.
    /// </summary>
    /// <param name="category">The category entity to add.</param>
    /// <returns>A task the categoryId of the new created category the asynchronous operation.</returns>
    Task<int> AddCategoryAsync(Category category);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
