using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IItemModifierGroupMapRepository
{

    /// <summary>
    /// Retrieves all itemModifierGroupMap as queryable.
    /// </summary>
    /// <returns>All itemModifierGroupMap as queryable.</returns>
    IQueryable<ItemModifierGroupMap> GetAllItemModifierGroupMapsAsQueryable();

    /// <summary>
    /// Retrieves a list of item-modifier group mappings for a specific item.
    /// </summary>
    /// <param name="itemId">The ID of the item to retrieve mappings for.</param>
    /// <returns>A task that returns a list of item-modifier group mappings.</returns>
    Task<List<ItemModifierGroupMap>> GetMappingByItemIdAsync(int itemId);

    /// <summary>
    /// Adds a new itemModifierGroupMap asynchronously in the database.
    /// </summary>
    /// <param name="itemModifierGroupMap">The itemModifierGroupMap entity to add.</param>
    /// <returns>A task that returns the ID of the created itemModifierGroupMap.</returns>
    Task<int> AddItemModifierGroupMapAsync(ItemModifierGroupMap itemModifierGroupMap);

    /// <summary>
    /// Removes itemModifierGroupMaps from the current context.
    /// </summary>
    void RemoveItemModifierGroupMaps(IEnumerable<ItemModifierGroupMap> itemModifierGroupMaps);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
