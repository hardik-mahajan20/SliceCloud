using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface ISectionService
{
    /// <summary>
    /// Retrieves all sections which is not deleted and order by section order.
    /// </summary>
    /// <returns>A collection of all sections as list in the form of sectionViewModel.</returns>
    Task<List<SectionViewModel>> GetAllSections();

    /// <summary>
    /// Retrieves a section by its ID asynchronously.
    /// </summary>
    /// <param name="sectionId">The ID of the section to retrieve.</param>
    /// <returns>A task that returns the section view model if found, otherwise null.</returns>
    Task<SectionViewModel> GetSectionByIdAsync(int sectionId);

    /// <summary>
    /// Checks if a section with the specified name exists, optionally excluding a specific section by ID.
    /// </summary>
    /// <param name="sectionName">The name of the section to check.</param>
    /// <param name="excludeSectionId">The ID of the section to exclude from the check (optional).</param>
    /// <returns>A task that returns true if a duplicate section exists, otherwise false.</returns>
    Task<bool> CheckDuplicateSectionNameAsync(string sectionName, int? excludeSectionId = null);

    /// <summary>
    /// Adds a new section asynchronously.
    /// </summary>
    /// <param name="sectionViewModel">The sectionViewModel to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> AddSectionAsync(SectionViewModel sectionViewModel);

    /// <summary>
    /// Updates a existing section asynchronously.
    /// </summary>
    /// <param name="sectionViewModel">The sectionViewModel to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> UpdateSectionAsync(SectionViewModel sectionViewModel);

    /// <summary>
    /// Deletes a section by its ID asynchronously.
    /// </summary>
    /// <param name="sectionId">The ID of the section to delete.</param>
    /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteSectionAsync(int sectionId);

    /// <summary>
    /// Updates the order of sections asynchronously.
    /// </summary>
    /// <param name="sortedSectionIds">The list of section IDs in the desired order.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateSectionOrderAsync(List<int> sortedSectionIds);
}
