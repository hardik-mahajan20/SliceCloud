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
}
