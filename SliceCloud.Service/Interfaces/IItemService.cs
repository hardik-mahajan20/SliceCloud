using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IItemService
{
    /// <summary>
    /// Retrieves a paginated list of items by group ID with an optional search query.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve items for.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchQuery">An optional search query to filter items.</param>
    /// <returns>A task that returns a paginated list of item view models.</returns>
    Task<PaginatedList<ItemViewModel>> GetPaginatedItemsByGroupIdAsync(int categoryId, int pageNumber, int pageSize, string searchQuery = "");

    /// <summary>
    /// Checks if an item with the specified name already exists, optionally excluding a specific item by ID.
    /// </summary>
    /// <param name="itemName">The name of the item to check.</param>
    /// <param name="itemId">The ID of the item to exclude from the check (optional).</param>
    /// <returns>True if a duplicate item exists, otherwise false.</returns>
    Task<bool> IsDuplicateItemAsync(string itemName, int? itemId = null);

    /// <summary>
    /// Adds a new menu item along with its image asynchronously.
    /// </summary>
    /// <param name="itemViewModel">The view model containing item details.</param>
    /// <param name="itemImage">The image file of the item.</param>
    /// <returns>A task that returns the ID of the newly added item.</returns>
    Task<int> AddMenuItemAsync(ItemViewModel itemViewModel, IFormFile? itemImage);

    /// <summary>
    /// Retrieves an item by its ID asynchronously.
    /// </summary>
    /// <param name="itemId">The ID of the item to retrieve.</param>
    /// <returns>The item view model if found, otherwise null.</returns>
    Task<ItemViewModel> GetItemByIdAsync(int itemId);
}
