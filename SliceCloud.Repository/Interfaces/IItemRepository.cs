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
    /// Adds a new menu item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>A task that returns the ID of the newly added item.</returns>
    Task<int> AddMenuItemAsync(Item item);
}
