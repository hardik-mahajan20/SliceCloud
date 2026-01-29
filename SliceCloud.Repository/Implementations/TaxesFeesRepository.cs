using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;

namespace SliceCloud.Repository.Implementations;

public class TaxesFeesRepository(SliceCloudContext sliceCloudContext) : ITaxesFeesRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllTaxes

    public async Task<List<Taxis>> GetAllTaxesAsync()
    {
        return await _sliceCloudContext.Taxes.Where(t => !t.IsDeleted ?? false).ToListAsync();
    }

    #endregion

    #region GetTaxesAndFees

    public async Task<PaginatedList<Taxis>> GetTaxesAndFeesAsync(string search, int page, int pageSize, string sortColumn, string sortDirection)
    {
        IQueryable<Taxis>? query = _sliceCloudContext.Taxes.Where(t => !(t.IsDeleted ?? false)).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string trimmedSearch = search.Trim().ToLower();
            query = query.Where(
                t =>
                    t.TaxName.ToLower().Contains(trimmedSearch)
                    || t.TaxType.ToLower().Contains(trimmedSearch)
            );
        }

        query = sortColumn switch
        {
            "TaxName"
              => sortDirection == "asc"
                  ? query.OrderBy(t => t.TaxName)
                  : query.OrderByDescending(t => t.TaxName),
            "Value"
              => sortDirection == "asc"
                  ? query.OrderBy(t => t.TaxValue)
                  : query.OrderByDescending(t => t.TaxValue),
            _
              => sortDirection == "asc"
                  ? query.OrderBy(t => t.TaxId)
                  : query.OrderByDescending(t => t.TaxId),
        };

        return await PaginatedList<Taxis>.CreateAsync(query, page, pageSize);
    }

    #endregion

    #region GetTaxById

    public async Task<Taxis?> GetTaxByIdAsync(int taxId)
    {
        return await _sliceCloudContext.Taxes.FirstOrDefaultAsync(t => t.TaxId == taxId);
    }

    #endregion

    #region AddTax

    public async Task<bool> AddTaxAsync(Taxis tax)
    {
        _sliceCloudContext.Taxes.Add(tax);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region UpdateTax

    public async Task<bool> UpdateTaxAsync(Taxis tax)
    {
        _sliceCloudContext.Taxes.Update(tax);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region IsTaxNameExists

    public async Task<bool> IsTaxNameExistsAsync(string taxName, int? taxId = null)
    {
        return await _sliceCloudContext.Taxes.AnyAsync(
            t => t.TaxName == taxName && (!taxId.HasValue || t.TaxId != taxId)
        );
    }

    #endregion

    #region GetDefaultTaxesForItems

    public List<ItemSpecificTaxViewModel> GetDefaultTaxesForItemsAsync(List<int> itemIds)
    {
        return _sliceCloudContext.Items
                  .Where(i => itemIds.Contains(i.ItemId) && i.IsDefaultTax == true)
                  .Select(
                      i =>
                          new ItemSpecificTaxViewModel
                          {
                              ItemId = i.ItemId,
                              Percentage = i.TaxPercentage,
                              TaxName = "Other"
                          }
                  )
                  .ToList();
    }

    #endregion
}
