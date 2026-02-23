using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface ISectionService
{
    /// <summary>
    /// Retrieves all sections which is not deleted and order by section order.
    /// </summary>
    /// <returns>A collection of all sections as list in the form of sectionviewmodel.</returns>
    Task<List<SectionViewModel>> GetAllSections();

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
}
