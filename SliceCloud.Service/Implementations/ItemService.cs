using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ItemService(IItemRepository itemRepository, IImageService imageService, ICurrentUserService currentUserService) : IItemService
{
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IImageService _imageService = imageService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    #region GetPaginatedItemsByGroupId
    
    public async Task<PaginatedList<ItemViewModel>> GetPaginatedItemsByGroupIdAsync(int categoryId, int pageNumber, int pageSize, string searchQuery = "")
    {
        IQueryable<Item>? query = _itemRepository.GetAllItemsAsQueryable().Where(item => item.CategoryId == categoryId && item.IsDeleted == false)
               .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            string trimmedSearch = searchQuery.Trim().ToLower();
            query = query.Where(
                item =>
                    (item.ItemName != null && item.ItemName.ToLower().Contains(trimmedSearch))
                    || (
                        item.ShortCode != null
                        && item.ShortCode.ToLower().Contains(trimmedSearch)
                    )
            );
        }

        int totalCount = await query.CountAsync();
        List<Item>? items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        List<ItemViewModel>? itemViewModels = items.Select(item =>
            {
                bool hasImage = !string.IsNullOrEmpty(item.ItemImage);

                return new ItemViewModel
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    ItemType = item.ItemType,
                    Rate = item.Rate,
                    Quantity = item.Quantity,
                    IsAvailable = item.IsAvailable ?? false,
                    ItemImg = hasImage
                        ? $"/images/uploads/{item.ItemImage}"
                        : "/images/dining-menu.png"
                };
            }).ToList();


        return new PaginatedList<ItemViewModel>(itemViewModels, totalCount, pageNumber, pageSize);
    }

    #endregion

    #region IsDuplicateItem

    public async Task<bool> IsDuplicateItemAsync(string itemName, int? itemId = null)
    {
        return await _itemRepository.GetAllItemsAsQueryable()
                                        .AnyAsync(item => item.ItemName.ToLower() == itemName.ToLower()
                                                        && (itemId == null || item.ItemId != itemId)
                                                        && item.IsDeleted == false);
    }

    #endregion

    #region AddMenuItem

    public async Task<int> AddMenuItemAsync(ItemViewModel model, IFormFile? itemImage)
    {
        string? itemImgPath = string.Empty;
        if (itemImage != null && itemImage.Length > 0)
        {
            itemImgPath = await _imageService.ImgPath(itemImage);
        }

        Item menuItem = new()
        {
            CategoryId = model.CategoryId,
            ItemName = model.ItemName,
            Rate = model.Rate,
            Quantity = model.Quantity,
            UnitId = model.UnitId,
            IsAvailable = model.IsAvailable,
            TaxPercentage = model.TaxPercentage,
            ShortCode = model.ShortCode,
            ItemType = model.ItemType ?? "Veg",
            IsFavorite = false,
            IsDefaultTax = model.IsDefaultTax,
            ItemImage = itemImgPath,
            Description = model.Description,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };
        return await _itemRepository.AddMenuItemAsync(menuItem);
    }

    #endregion
}
