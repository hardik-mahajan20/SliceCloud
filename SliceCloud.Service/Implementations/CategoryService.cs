using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;


    public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
    {
        List<Category> categories = await _categoryRepository.GetAllCategoriesAsQueryable().Where(c => c.IsDeleted == false)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

        return categories.Select(item => new CategoryViewModel()
        {
            CategoryId = item.CategoryId,
            CategoryName = item.CategoryName,
            Description = item.Description
        }).ToList();
    }
}
