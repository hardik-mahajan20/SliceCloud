using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

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

    /// <summary>
    /// Retrieves a paginated list of modifiers by modifiergroup ID with an optional search query.
    /// </summary>
    /// <param name="modifierGroupId">The ID of the modifierGruop to retrieve modifiers for.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The number of modifiers per page.</param>
    /// <param name="searchQuery">An optional search query to filter modifiers.</param>
    /// <returns>A task that returns a paginated list of modifier view models.</returns>
    Task<PaginatedList<ModifierViewModel>> GetPaginatedModifiersByModifierGroupId(int modifierGroupId, int pageNumber, int pageSize, string searchQuery = "");

    /// <summary>
    /// Adds a new modifier asynchronously.
    /// </summary>
    /// <param name="modifierSectionViewModel">The modifier view model to add.</param>
    /// <returns>A task the modifier of the new created modifier   asynchronous operation.</returns>
    Task<int> AddModifierAsync(ModifierSectionViewModel modifierSectionViewModel);

    /// <summary>
    /// Retrieves a modifier by its ID asynchronously.
    /// </summary>
    /// <param name="modifierId">The ID of the modifier to retrieve.</param>
    /// <returns>A task that returns the modifier view model if found, otherwise null.</returns>
    Task<ModifierViewModel> GetModifierByIdAsync(int modifierId);
}
