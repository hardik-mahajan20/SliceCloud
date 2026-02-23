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
    /// <param name="id">The ID of the tax to retrieve.</param>
    /// <returns>A task that returns the tax if found, otherwise null.</returns>
    Task<Taxis?> GetTaxByIdAsync(int taxId);

    /// <summary>
    /// Adds a new tax asynchronously.
    /// </summary>
    /// <param name="tax">The tax to add.</param>
    /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
    Task<bool> AddTaxAsync(Taxis tax);

    /// <summary>
    /// Updates an existing tax.
    /// </summary>
    /// <param name="tax">The tax to update.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    Task<bool> UpdateTaxAsync(Taxis tax);
}
