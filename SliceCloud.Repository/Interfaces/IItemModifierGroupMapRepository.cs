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
}
