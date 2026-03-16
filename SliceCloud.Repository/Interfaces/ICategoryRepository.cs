using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ICategoryRepository
{
    /// <summary>
    /// Retrieves all categories as queryable.
    /// </summary>
    /// <returns>All categories as queryable.</returns>
    IQueryable<Category> GetAllCategoriesAsQueryable();

    /// <summary>
    /// Retrieves a category by its ID asynchronously.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>A task that returns the category if found in the database, otherwise null.</returns>
    Task<Category?> GetCategoryByIdAsync(int categoryId);

    /// <summary>
    /// Adds a new category asynchronously in the database.
    /// </summary>
    /// <param name="category">The category entity to add.</param>
    /// <returns>A task that returns the ID of the created category.</returns>
    Task<int> AddCategoryAsync(Category category);

    /// <summary>
    /// Updates an existing category asynchronously in the database.
    /// </summary>
    /// <param name="category">The category to update.</param>
    /// <returns>A task that returns the ID of the updated category.</returns>
    Task<int> UpdateCategoryAsync(Category category);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
