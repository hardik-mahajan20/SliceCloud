using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IItemModifierGroupMapService
{
    /// <summary>
    /// Adds a new item-modifier group mapping asynchronously.
    /// </summary>
    /// <param name="itemModifierGroupMapViewModel">The view model containing the item-modifier group mapping details.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddItemModifierGroupMapAsync(ItemModifierGroupMapViewModel itemModifierGroupMapViewModel);
}
