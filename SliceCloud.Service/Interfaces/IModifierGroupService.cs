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
}
