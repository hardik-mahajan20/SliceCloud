
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
    /// Adds a new section asynchronously.
    /// </summary>
    /// <param name="section">The section to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<bool> AddSectionAsync(Section section);
}
