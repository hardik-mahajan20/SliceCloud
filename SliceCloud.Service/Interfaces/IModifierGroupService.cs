using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface IModifierGroupService
{
    /// <summary>
    /// Retrieves all modifier groups.
    /// </summary>
    /// <returns>A collection of modifier group view models.</returns>
    Task<List<ModifierGroupViewModel>> GetAllModifierGroupsAsync();

    /// <summary>
    /// Retrieves a list of modifier groups by their IDs.
    /// </summary>
    /// <param name="modifierGroupIds">The list of modifier group IDs to retrieve.</param>
    /// <returns>A list of modifier groups.</returns>
    Task<List<ModifierGroup>> GetModifierGroupsByIdsAsync(List<int> modifierGroupIds);

    /// <summary>
    /// Retrieves a modifier group by its ID asynchronously.
    /// </summary>
    /// <param name="modifierGroupId">The ID of the modifier group to retrieve.</param>
    /// <returns>A task that returns the modifier group view model if found, otherwise null.</returns>
    Task<ModifierGroupViewModel> GetModifierGroupByIdAsync(int modifierGroupId);

    /// <summary>
    /// Updates the order of modifier groups asynchronously.
    /// </summary>
    /// <param name="orderedModifierGroupIds">The list of modifier group IDs in the desired order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateModifierGroupOrderAsync(List<int> orderedModifierGroupIds);
}
