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
    
}
