using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface ITaxesFeesRepository
{
    /// <summary>
    /// Retrieves all taxes as queryable.
    /// </summary>
    /// <returns>All taxes as queryable.</returns>
    IQueryable<Taxis> GetAllTaxisAsQueryable();

    /// <summary>
    /// Retrieves a tax by its ID asynchronously.
    /// </summary>
    /// <param name="taxId">The ID of the tax to retrieve.</param>
    /// <returns>A task that returns the tax if found in the database, otherwise null.</returns>
    Task<Taxis?> GetTaxByIdAsync(int taxId);

    /// <summary>
    /// Adds a new tax asynchronously in the database.
    /// </summary>
    /// <param name="tax">The tax entity to add.</param>
    /// <returns>A task that returns the ID of the created tax.</returns>
    Task<int> AddTaxAsync(Taxis tax);

    /// <summary>
    /// Updates an existing tax asynchronously in the database.
    /// </summary>
    /// <param name="tax">The tax to update.</param>
    /// <returns>A task that returns the ID of the updated tax.</returns>
    Task<int> UpdateTaxAsync(Taxis tax);
}
