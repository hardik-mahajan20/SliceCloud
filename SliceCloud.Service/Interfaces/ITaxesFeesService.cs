using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Service.Interfaces;

public interface ITaxesFeesService
{
    /// <summary>
    /// Retrieves a paginated list of taxes and fees based on search criteria and sorting options.
    /// </summary>
    /// <param name="search">The search term to filter taxes and fees.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of taxes and fees per page.</param>
    /// <param name="sortColumn">The column to sort the results by.</param>
    /// <param name="sortDirection">The direction of sorting (e.g., ascending or descending).</param>
    /// <returns>A task that returns a paginated list of taxes and fees view models.</returns>
    public Task<PaginatedList<TaxesFeesViewModel>> GetTaxesAndFeesAsync(string search, int page, int pageSize, string sortColumn, string sortDirection);

    /// <summary>
    /// Checks if a tax name already exists, optionally excluding a specific tax by ID.
    /// </summary>
    /// <param name="taxName">The name of the tax to check.</param>
    /// <param name="taxId">The ID of the tax to exclude from the check (optional).</param>
    /// <returns>A task that returns true if the tax name exists, otherwise false.</returns>
    Task<bool> IsDuplicateTaxNameAsync(string taxName, int? taxId = null);

    /// <summary>
    /// Adds a new tax asynchronously.
    /// </summary>
    /// <param name="model">The view model containing tax details.</param>
    /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
    Task<bool> AddTaxAsync(TaxesFeesViewModel taxesFeesViewModel);

    /// <summary>
    /// Retrieves a tax by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the tax to retrieve.</param>
    /// <returns>A task that returns the view model containing tax details if found, otherwise null.</returns>
    Task<TaxesFeesViewModel> GetTaxByIdAsync(int id);

    /// <summary>
    /// Updates an existing tax.
    /// </summary>
    /// <param name="model">The view model containing updated tax details.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    Task<bool> UpdateTaxAsync(TaxesFeesViewModel model);

    /// <summary>
    /// Deletes a tax by its ID asynchronously.
    /// </summary>
    /// <param name="taxId">The ID of the tax to delete.</param>
    /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteTaxAsync(int taxId);

    /// <summary>
    /// Toggles the value of a specific field for a tax asynchronously.
    /// </summary>
    /// <param name="taxId">The ID of the tax to update.</param>
    /// <param name="isChecked">The new value for the field.</param>
    /// <param name="field">The name of the field to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ToggleTaxFieldAsync(int taxId, bool isChecked, string field);

    /// <summary>
    /// Gets the list of enabled tax asynchronously.
    /// </summary>
    /// <returns>A task which return the list of the enable taxes.</returns>
    Task<List<TaxViewModel>> GetEnabledTaxesAsync();

    /// <summary>
    /// Gets the list of the enabled taxies on the provided itemIds asynchronously.
    /// </summary>
    /// <param name="itemIds">The list ID of on which tax should be.</param>
    /// <returns>A task that returns item specific tax details.</returns>
    Task<List<ItemSpecificTaxViewModel>> GetDefaultItemTaxesAsync(List<int> itemIds);
}
