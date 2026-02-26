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


    /// <summary>
    /// Retrieves a category by its ID asynchronously.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>A task that returns the category if found in the database, otherwise null.</returns>
    Task<Category?> GetCategoryByIdAsync(int categoryId);


    /// <summary>
    /// Updates an existing category asynchronously in the database.
    /// </summary>
    /// <param name="category">The category to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateCategoryAsync(Category category);
}
