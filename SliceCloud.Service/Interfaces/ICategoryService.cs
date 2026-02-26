using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface ICategoryService
{
    /// <summary>
    /// Retrieves all categories asynchronously.
    /// </summary>
    /// <returns>A collection of category view models asynchronously.</returns>
    Task<List<CategoryViewModel>> GetAllCategoriesAsync();

    /// <summary>
    /// Updates the order of categories asynchronously.
    /// </summary>
    /// <param name="sortedCategoryIds">The list of category IDs in the desired order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateCategoryOrderAsync(List<int> sortedCategoryIds);

    /// <summary>
    /// Adds a new category asynchronously.
    /// </summary>
    /// <param name="categoryViewModel">The category view model to add.</param>
    /// <returns>A task the category of the new created category   asynchronous operation.</returns>
    Task<int> AddCategoryAsync(CategoryViewModel categoryViewModel);

    /// <summary>
    /// Retrieves a category by its ID asynchronously.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <returns>A task that returns the category view model if found, otherwise null.</returns>
    Task<CategoryViewModel> GetCategoryByIdAsync(int categoryId);

    /// <summary>
    /// Updates an existing category asynchronously.
    /// </summary>
    /// <param name="categoryViewModel">The category view model to update.</param>
    /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
    Task<bool> UpdateAsync(CategoryViewModel categoryViewModel);
}
