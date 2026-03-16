
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ISectionRepository
{
    /// <summary>
    /// Retrieves all sections as queryable.
    /// </summary>
    /// <returns>All sections as queryable.</returns>
    IQueryable<Section> GetAllSectionsAsQueryable();

    /// <summary>
    /// Retrieves a section by its ID asynchronously.
    /// </summary>
    /// <param name="sectionId">The ID of the section to retrieve.</param>
    /// <returns>A task that returns the section if found in the database, otherwise null.</returns>
    Task<Section?> GetSectionByIdAsync(int sectionId);

    /// <summary>
    /// Adds a new section asynchronously in the database.
    /// </summary>
    /// <param name="section">The section entity to add.</param>
    /// <returns>A task that returns the ID of the created section.</returns>
    Task<int> AddSectionAsync(Section section);

    /// <summary>
    /// Updates an existing section asynchronously in the database.
    /// </summary>
    /// <param name="section">The section to update.</param>
    /// <returns>A task that returns the ID of the updated section.</returns>
    Task<int> UpdateSectionAsync(Section section);

    /// <summary>
    /// Saves changes to the data source asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync();
}
