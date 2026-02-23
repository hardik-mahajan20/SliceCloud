using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class TaxesFeesRepository(SliceCloudContext sliceCloudContext) : ITaxesFeesRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllTaxes

    public IQueryable<Taxis> GetAllTaxisAsQueryable()
    {
        return _sliceCloudContext.Taxes.AsQueryable();
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
}
