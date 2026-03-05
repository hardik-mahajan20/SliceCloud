using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IItemRepository
{
    /// <summary>
    /// Retrieves all items as queryable.
    /// </summary>
    /// <returns>All items as queryable.</returns>
    IQueryable<Item> GetAllItemsAsQueryable();

    /// <summary>
    /// Adds a new menu item asynchronously.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A task that returns the ID of the newly added item.</returns>
    Task<int> AddMenuItemAsync(Item item);

    /// <summary>
    /// Retrieves an item by its ID asynchronously.
    /// </summary>
    /// <param name="itemId">The ID of the item to retrieve.</param>
    /// <returns>The item if found, otherwise null.</returns>
    Task<Item?> GetItemByIdAsync(int itemId);

    /// <summary>
    /// Updates an existing menu item asynchronously.
    /// </summary>
    /// <param name="item">The menu item to update.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    Task<bool> UpdateMenuItemAsync(Item item);
}
