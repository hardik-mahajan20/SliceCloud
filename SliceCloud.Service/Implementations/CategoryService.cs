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

    public async Task UpdateCategoryOrderAsync(List<int> sortedCategoryIds)
    {
        List<Category>? categories = await _categoryRepository.GetAllCategoriesAsQueryable()
                                .Where(s => sortedCategoryIds.Contains(s.CategoryId) && !(s.IsDeleted ?? false))
                                    .ToListAsync();

        Dictionary<int, Category>? categoryDictionary = categories.ToDictionary(s => s.CategoryId);

        for (int i = 0; i < sortedCategoryIds.Count; i++)
        {
            if (categoryDictionary.TryGetValue(sortedCategoryIds[i], out var section))
            {
                section.SortOrder = i + 1;
            }
        }

        await _categoryRepository.SaveChangesAsync();
    }
}
