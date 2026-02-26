using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CategoryService(ICategoryRepository categoryRepository, ICurrentUserService currentUserService) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    private readonly ICurrentUserService _currentUserService = currentUserService;

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

    public async Task<int> AddCategoryAsync(CategoryViewModel categoryViewModel)
    {
        bool isCategoryNameExists = await _categoryRepository.GetAllCategoriesAsQueryable().AsNoTracking()
                        .AnyAsync(c => c.CategoryName == categoryViewModel.CategoryName && (c.IsDeleted == false));

        if (isCategoryNameExists)
        {
            throw new InvalidOperationException("A category with the same name already exists.");
        }

        int maxOrder = await _categoryRepository.GetAllCategoriesAsQueryable().Where(s => !s.IsDeleted == false).Select(s => (int?)s.SortOrder).MaxAsync() ?? 0;

        Category category = new()
        {
            CategoryName = categoryViewModel.CategoryName ?? string.Empty,
            Description = categoryViewModel.Description,
            IsDeleted = false,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow,
            SortOrder = maxOrder + 1
        };

        return await _categoryRepository.AddCategoryAsync(category);
    }

    public async Task<CategoryViewModel> GetCategoryByIdAsync(int categoryId)
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

        if (category == null)
        {
            return new CategoryViewModel();
        }

        return new CategoryViewModel
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description
        };
    }

    public async Task<bool> UpdateAsync(CategoryViewModel categoryViewModel)
    {
        Category? category = await _categoryRepository.GetCategoryByIdAsync(categoryViewModel.CategoryId);

        if (category == null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        bool isCategoryNameExists = await _categoryRepository.GetAllCategoriesAsQueryable().AsNoTracking()
                      .AnyAsync(c => c.CategoryName == categoryViewModel.CategoryName && (c.IsDeleted == false));

        if (isCategoryNameExists)
        {
            throw new InvalidOperationException("A category with the same name already exists.");
        }

        category.CategoryName = categoryViewModel.CategoryName ?? string.Empty;
        category.Description = categoryViewModel.Description;
        category.ModifiedBy = _currentUserService.UserId;
        category.ModifiedAt = DateTime.UtcNow;

        return await _categoryRepository.UpdateCategoryAsync(category);
    }
}
