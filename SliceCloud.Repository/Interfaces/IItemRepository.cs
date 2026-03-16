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
    /// Retrieves a item by its ID asynchronously.
    /// </summary>
    /// <param name="itemId">The ID of the item to retrieve.</param>
    /// <returns>A task that returns the item if found in the database, otherwise null.</returns>
    Task<Item?> GetItemByIdAsync(int itemId);

    /// <summary>
    /// Adds a new item asynchronously in the database.
    /// </summary>
    /// <param name="item">The item entity to add.</param>
    /// <returns>A task that returns the ID of the created item.</returns>
    Task<int> AddMenuItemAsync(Item item);

    /// <summary>
    /// Updates an existing item asynchronously in the database.
    /// </summary>
    /// <param name="item">The item to update.</param>
    /// <returns>A task that returns the ID of the updated item.</returns>
    Task<int> UpdateMenuItemAsync(Item item);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
