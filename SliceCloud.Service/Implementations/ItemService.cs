using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ItemService(IItemRepository itemRepository) : IItemService
{
    private readonly IItemRepository _itemRepository = itemRepository;

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
                    Isavailable = item.IsAvailable ?? false,
                    ItemImg = hasImage
                        ? $"/images/uploads/{item.ItemImage}"
                        : "/images/dining-menu.png"
                };
            }).ToList();


        return new PaginatedList<ItemViewModel>(itemViewModels, totalCount, pageNumber, pageSize);
    }

}
