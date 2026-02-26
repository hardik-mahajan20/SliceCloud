using Microsoft.EntityFrameworkCore;
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

    #region GetCategoryById

    public async Task<Category?> GetCategoryByIdAsync(int categoryId)
    {
        return await _sliceCloudContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
    }

    #endregion

    #region UpdateCategory

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        _sliceCloudContext.Categories.Update(category);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
