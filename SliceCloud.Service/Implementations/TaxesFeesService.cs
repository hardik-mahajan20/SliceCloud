using System.Threading.Tasks;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class TaxesFeesService(ITaxesFeesRepository taxesFeesRepository, ICurrentUserService currentUserService) : ITaxesFeesService
{
    private readonly ITaxesFeesRepository _taxesFeesRepository = taxesFeesRepository;

    private readonly ICurrentUserService _currentUserService = currentUserService;

    #region GetAllTaxes

    public async Task<List<TaxesFeesViewModel>> GetAllTaxesAsync()
    {
        List<Taxis>? taxes = await _taxesFeesRepository.GetAllTaxesAsync();

        return taxes.Select(t => new TaxesFeesViewModel
        {
            TaxId = t.TaxId,
            TaxName = t.TaxName,
            TaxType = t.TaxType,
            IsEnabled = t.IsEnabled ?? false,
            IsDefault = t.IsDefault ?? false,
            IsInclusive = t.IsInclusive ?? false,
            TaxValue = (decimal?)t.TaxValue
        }).ToList();
    }

    #endregion

    #region GetTaxesAndFees

    public async Task<PaginatedList<TaxesFeesViewModel>> GetTaxesAndFeesAsync(string search, int page, int pageSize, string sortColumn, string sortDirection)
    {
        PaginatedList<Taxis>? taxes = await _taxesFeesRepository.GetTaxesAndFeesAsync(search, page, pageSize, sortColumn, sortDirection);

        List<TaxesFeesViewModel>? taxesViewModel = taxes.Select(t => new TaxesFeesViewModel
        {
            TaxId = t.TaxId,
            TaxName = t.TaxName,
            TaxType = t.TaxType,
            IsEnabled = t.IsEnabled ?? false,
            IsDefault = t.IsDefault ?? false,
            IsInclusive = t.IsInclusive ?? false,
            TaxValue = (decimal?)t.TaxValue
        }).ToList();

        return new PaginatedList<TaxesFeesViewModel>(taxesViewModel, taxes.TotalItems, page, pageSize);
    }

    #endregion

    #region IsDuplicateTaxName

    public async Task<bool> IsDuplicateTaxNameAsync(string taxName, int? taxId = null)
    {
        return await _taxesFeesRepository.IsTaxNameExistsAsync(taxName, taxId);
    }

    #endregion

    #region AddTaxAsync

    public async Task<bool> AddTaxAsync(TaxesFeesViewModel model)
    {
        Taxis tax = new()
        {
            TaxName = model.TaxName ?? string.Empty,
            TaxType = model.TaxType ?? string.Empty,
            TaxValue = (int)(model.TaxValue ?? 0),
            IsEnabled = model.IsEnabled,
            IsDefault = model.IsDefault,
            CreatedBy = _currentUserService.UserId
        };

        return await _taxesFeesRepository.AddTaxAsync(tax);
    }

    #endregion

    #region GetTaxById

    public async Task<TaxesFeesViewModel> GetTaxByIdAsync(int id)
    {
        Taxis? tax = await _taxesFeesRepository.GetTaxByIdAsync(id);
        if (tax == null) return new TaxesFeesViewModel();

        return new TaxesFeesViewModel
        {
            TaxId = tax.TaxId,
            TaxName = tax.TaxName,
            TaxType = tax.TaxType,
            TaxValue = (decimal?)tax.TaxValue,
            IsEnabled = tax.IsEnabled ?? false,
            IsDefault = tax.IsDefault ?? false
        };
    }

    #endregion

    #region UpdateTax

    public async Task<bool> UpdateTaxAsync(TaxesFeesViewModel model)
    {
        Taxis? tax = await _taxesFeesRepository.GetTaxByIdAsync(model.TaxId);
        if (tax == null) return false;

        tax.TaxName = model.TaxName ?? string.Empty;
        tax.TaxType = model.TaxType ?? string.Empty;
        tax.TaxValue = (int)(model.TaxValue ?? 0);
        tax.IsEnabled = model.IsEnabled;
        tax.IsDefault = model.IsDefault;
        tax.ModifiedBy = _currentUserService.UserId;
        tax.ModifiedAt = DateTime.UtcNow;

        return await _taxesFeesRepository.UpdateTaxAsync(tax);
    }

    #endregion

    #region DeleteTax

    public async Task<bool> DeleteTaxAsync(int taxId)
    {
        Taxis? taxis = await _taxesFeesRepository.GetTaxByIdAsync(taxId) ?? throw new Exception("Tax not found");

        taxis.IsDeleted = true;
        taxis.ModifiedBy = _currentUserService.UserId;
        taxis.ModifiedAt = DateTime.UtcNow;

        return await _taxesFeesRepository.UpdateTaxAsync(taxis);
    }

    #endregion

    #region ToggleTaxField

    public async Task ToggleTaxFieldAsync(int taxId, bool isChecked, string field)
    {
        Taxis? taxis = await _taxesFeesRepository.GetTaxByIdAsync(taxId) ?? throw new Exception("Tax not found");

        switch (field)
        {
            case "IsEnabled":
                taxis.IsEnabled = isChecked;
                break;
            case "IsDefault":
                taxis.IsDefault = isChecked;
                break;
            case "IsInclusive":
                taxis.IsInclusive = isChecked;
                break;
            default:
                throw new Exception("Invalid field type.");
        }
        taxis.ModifiedBy = _currentUserService.UserId;
        taxis.ModifiedAt = DateTime.UtcNow;
        await _taxesFeesRepository.UpdateTaxAsync(taxis);
    }

    #endregion

    #region GetEnabledTaxes

    public async Task<List<TaxViewModel>> GetEnabledTaxesAsync()
    {
        List<Taxis>? taxes = await _taxesFeesRepository.GetAllTaxesAsync();

        return taxes
            .Where(t => t.IsEnabled == true)
            .Select(t => new TaxViewModel
            {
                TaxId = t.TaxId,
                TaxName = t.TaxName,
                Amount = (decimal)t.TaxValue,
                TaxType = t.TaxType
            })
            .ToList();
    }

    #endregion

    #region GetDefaultItemTaxes

    public List<ItemSpecificTaxViewModel> GetDefaultItemTaxesAsync(List<int> itemIds)
    {
        if (itemIds == null || !itemIds.Any())
            return new List<ItemSpecificTaxViewModel>();

        return _taxesFeesRepository.GetDefaultTaxesForItemsAsync(itemIds);
    }

    #endregion
}
