using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class CategoryRepository(SliceCloudContext sliceCloudContext) : ICategoryRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllCategoriesAsync

    public IQueryable<Category> GetAllCategoriesAsQueryable()
    {
        return _sliceCloudContext.Categories.AsQueryable();
    }

    #endregion

    #region AddCategory

    public async Task<int> AddCategoryAsync(Category category)
    {
        await _sliceCloudContext.Categories.AddAsync(category);
        await _sliceCloudContext.SaveChangesAsync();
        return category.CategoryId;
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
