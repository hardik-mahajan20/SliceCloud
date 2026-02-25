using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface ICategoryService
{
    /// <summary>
    /// Retrieves all categories asynchronously.
    /// </summary>
    /// <returns>A collection of category view models asynchronously.</returns>
    Task<List<CategoryViewModel>> GetAllCategoriesAsync();
}
