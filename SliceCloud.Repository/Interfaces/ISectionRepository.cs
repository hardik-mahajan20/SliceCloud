
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ISectionRepository
{
    /// <summary>
    /// Retrieves all sections as queryable.
    /// </summary>
    /// <returns>A collection of all sections as queryable.</returns>
    IQueryable<Section> GetAllSectionsAsQueryable();

    /// <summary>
    /// Retrieves a section by its ID asynchronously.
    /// </summary>
    /// <param name="sectionId">The ID of the section to retrieve.</param>
    /// <returns>A task that returns the section if found, otherwise null.</returns>
    Task<Section?> GetSectionByIdAsync(int sectionId);

    /// <summary>
    /// Adds a new section asynchronously.
    /// </summary>
    /// <param name="section">The section to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> AddSectionAsync(Section section);

    /// <summary>
    /// Updates a existing section asynchronously.
    /// </summary>
    /// <param name="section">The section to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> UpdateSectionAsync(Section section);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
