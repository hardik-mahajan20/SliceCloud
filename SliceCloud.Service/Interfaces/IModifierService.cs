using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Interfaces;

public interface IModifierService
{
    /// <summary>
    /// Retrieves a list of modifiers by multiple modifier group IDs.
    /// </summary>
    /// <param name="modifierGroupIds">The list of modifier group IDs to retrieve modifiers for.</param>
    /// <returns>A list of modifiers belonging to the specified groups.</returns>
    Task<List<Modifier>> GetModifiersByGroupIdsAsync(List<int> modifierGroupIds);

    /// <summary>
    /// Retrieves all modifier groups asynchronously.
    /// </summary>
    /// <returns>A list of all modifier groups.</returns>
    Task<List<ModifierGroup>> GetAllModifierGroupsAsync();
}
