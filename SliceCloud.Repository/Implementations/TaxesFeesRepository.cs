using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class TaxesFeesRepository(SliceCloudContext sliceCloudContext) : ITaxesFeesRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllTaxisAsQueryable

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

    public async Task<int> AddTaxAsync(Taxis tax)
    {
        await _sliceCloudContext.Taxes.AddAsync(tax);
        await _sliceCloudContext.SaveChangesAsync();
        return tax.TaxId;
    }

    #endregion

    #region UpdateTax

    public async Task<int> UpdateTaxAsync(Taxis tax)
    {
        _sliceCloudContext.Taxes.Update(tax);
        await _sliceCloudContext.SaveChangesAsync();
        return tax.TaxId;
    }

    #endregion

}
