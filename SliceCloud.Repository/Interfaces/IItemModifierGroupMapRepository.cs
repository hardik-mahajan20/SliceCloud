using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IItemModifierGroupMapRepository
{
    /// <summary>
    /// Adds a new item-modifier group mapping to the database.
    /// </summary>
    /// <param name="itemModifierGroupMap">The item-modifier group mapping to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddItemModifierGroupMapAsync(ItemModifierGroupMap itemModifierGroupMap);

    /// <summary>
    /// Retrieves a list of item-modifier group mappings for a specific item.
    /// </summary>
    /// <param name="itemId">The ID of the item to retrieve mappings for.</param>
    /// <returns>A task that returns a list of item-modifier group mappings.</returns>
    Task<List<ItemModifierGroupMap>> GetMappingByItemIdAsync(int itemId);

    /// <summary>
    /// Retrieves all itemModifierGroupMaps as queryable.
    /// </summary>
    /// <returns>All itemModifierGroupMaps as queryable.</returns>
    IQueryable<ItemModifierGroupMap> GetAllItemModifierGroupMapsAsQueryable();

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Removes itemModifierGroupMaps from the current context.
    /// </summary>
    void RemoveItemModifierGroupMaps(IEnumerable<ItemModifierGroupMap> itemModifierGroupMaps);
}
