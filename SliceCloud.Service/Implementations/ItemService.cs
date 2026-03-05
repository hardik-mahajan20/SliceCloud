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

    #region GetItemById

    public async Task<ItemViewModel> GetItemByIdAsync(int itemId)
    {
        Item? item = await _itemRepository.GetItemByIdAsync(itemId);
        if (item == null) return new ItemViewModel();

        return new ItemViewModel
        {
            ItemId = item.ItemId,
            CategoryId = item.CategoryId,
            ItemName = item.ItemName,
            ItemType = item.ItemType,
            Rate = item.Rate,
            Quantity = item.Quantity,
            UnitId = item.UnitId,
            IsAvailable = item.IsAvailable ?? false,
            IsDefaultTax = item.IsDefaultTax ?? false,
            TaxPercentage = item.TaxPercentage,
            ShortCode = item.ShortCode,
            Description = item.Description,
            ItemImg = item.ItemImage,
        };
    }

    #endregion

    #region UpdateMenuItem

    public async Task<bool> UpdateMenuItemAsync(EditMenuItemViewModel model, IFormFile? itemImage)
    {
        string? itemImgPath = await _imageService.ImgPath(itemImage);
        Item? menuItem = await _itemRepository.GetItemByIdAsync(model.ItemId);
        if (menuItem == null) return false;

        menuItem.ItemName = model.ItemName;
        menuItem.CategoryId = model.CategoryId;
        menuItem.ItemType = model.ItemType ?? "Veg";
        menuItem.Rate = model.Rate;
        menuItem.Quantity = model.Quantity;
        menuItem.UnitId = model.UnitId;
        menuItem.IsAvailable = model.IsAvailable;
        menuItem.IsDefaultTax = model.IsDefaultTax;
        menuItem.TaxPercentage = model.TaxPercentage;
        menuItem.ShortCode = model.ShortCode;
        menuItem.Description = model.Description;
        menuItem.ItemImage = model.ItemImg;
        menuItem.ItemImage = itemImgPath;
        menuItem.ModifiedAt = DateTime.UtcNow;
        menuItem.ModifiedBy = _currentUserService.UserId;

        return await _itemRepository.UpdateMenuItemAsync(menuItem);
    }

    #endregion

    #region DeleteItem

    public async Task<bool> DeleteItemAsync(int itemId)
    {
        Item? item = await _itemRepository.GetItemByIdAsync(itemId);
        if (item is null)
        {
            return false;
        }
        item.IsDeleted = true;
        item.ModifiedAt = DateTime.UtcNow;
        item.ModifiedBy = _currentUserService.UserId;

        return await _itemRepository.UpdateMenuItemAsync(item);
    }

    #endregion

    #region GetAllItemIds

    public async Task<List<int>> GetAllItemIdsAsync(int categoryId)
    {
        return await _itemRepository.GetAllItemsAsQueryable()
                                        .Where(item => item.IsDeleted == false && item.CategoryId == categoryId)
                                            .Select(item => item.ItemId)
                                                .ToListAsync();
    }

    #endregion

    #region DeleteMultipleMultipleItem

    public async Task<bool> DeleteMultipleMenuItemAsync(List<int> itemIds)
    {
        List<Item>? items = await _itemRepository.GetAllItemsAsQueryable().Where(i => itemIds.Contains(i.ItemId)).ToListAsync();

        if (items.Any())
        {
            foreach (Item item in items)
            {
                item.IsDeleted = true;
                item.ModifiedAt = DateTime.UtcNow;
                item.ModifiedBy = _currentUserService.UserId;
            }
        }
        return await _itemRepository.SaveChangesAsync() > 0;
    }

    #endregion
}
