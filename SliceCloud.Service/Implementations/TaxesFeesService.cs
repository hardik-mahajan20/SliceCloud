using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Constants;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class TaxesFeesService(ITaxesFeesRepository taxesFeesRepository, ICurrentUserService currentUserService, IItemRepository itemRepository) : ITaxesFeesService
{
    private readonly ITaxesFeesRepository _taxesFeesRepository = taxesFeesRepository;

    private readonly ICurrentUserService _currentUserService = currentUserService;

    private readonly IItemRepository _itemRepository = itemRepository;

    #region GetAllTaxes

    public async Task<List<TaxesFeesViewModel>> GetAllTaxesAsync()
    {
        // List<Taxis>? taxes = await _taxesFeesRepository.GetAllTaxesAsync();
        List<Taxis>? taxes = await _taxesFeesRepository.GetAllTaxisAsQueryable().Where(t => !t.IsDeleted ?? false).ToListAsync();

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
        IQueryable<Taxis>? query = _taxesFeesRepository.GetAllTaxisAsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string trimmedSearch = search.Trim().ToLower();
            query = query.Where(
                t =>
                    t.TaxName.ToLower() == trimmedSearch
                    || t.TaxType.ToLower() == trimmedSearch
            );
        }

        query = sortColumn switch
        {
            TaxConstants.TAX_NAME
              => sortDirection == GeneralConstants.ASCENDING
                  ? query.OrderBy(t => t.TaxName)
                  : query.OrderByDescending(t => t.TaxName),
            TaxConstants.TAX_VALUE
              => sortDirection == GeneralConstants.ASCENDING
                  ? query.OrderBy(t => t.TaxValue)
                  : query.OrderByDescending(t => t.TaxValue),
            _
              => sortDirection == GeneralConstants.ASCENDING
                  ? query.OrderBy(t => t.TaxId)
                  : query.OrderByDescending(t => t.TaxId),
        };

        PaginatedList<Taxis>? taxes = await PaginatedList<Taxis>.CreateAsync(query, page, pageSize);

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
        IQueryable<Taxis>? query = _taxesFeesRepository.GetAllTaxisAsQueryable();

        if (taxId is not null)
        {
            return await query.AnyAsync(t => t.TaxName == taxName && t.TaxId != taxId);
        }
        else
        {
            return await query.AnyAsync(t => t.TaxName == taxName);
        }
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

        return await _taxesFeesRepository.AddTaxAsync(tax) > 0;
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

        return await _taxesFeesRepository.UpdateTaxAsync(tax) > 0;
    }

    #endregion

    #region DeleteTax

    public async Task<bool> DeleteTaxAsync(int taxId)
    {
        Taxis? taxis = await _taxesFeesRepository.GetTaxByIdAsync(taxId) ?? throw new Exception(ErrorConstants.TAX_NOT_FOUND);

        taxis.IsDeleted = true;
        taxis.ModifiedBy = _currentUserService.UserId;
        taxis.ModifiedAt = DateTime.UtcNow;

        return await _taxesFeesRepository.UpdateTaxAsync(taxis) > 0;
    }

    #endregion

    #region ToggleTaxField

    public async Task ToggleTaxFieldAsync(int taxId, bool isChecked, string field)
    {
        Taxis? taxis = await _taxesFeesRepository.GetTaxByIdAsync(taxId) ?? throw new Exception(ErrorConstants.TAX_NOT_FOUND);

        switch (field)
        {
            case TaxConstants.TAX_IS_ENABLE:
                taxis.IsEnabled = isChecked;
                break;
            case TaxConstants.TAX_IS_DEFAULT:
                taxis.IsDefault = isChecked;
                break;
            case TaxConstants.TAX_IS_INCLUSIVE:
                taxis.IsInclusive = isChecked;
                break;
            default:
                throw new Exception(ErrorConstants.INVALID_FIELD_TYPE);
        }
        taxis.ModifiedBy = _currentUserService.UserId;
        taxis.ModifiedAt = DateTime.UtcNow;
        await _taxesFeesRepository.UpdateTaxAsync(taxis);
    }

    #endregion

    #region GetEnabledTaxes

    public async Task<List<TaxViewModel>> GetEnabledTaxesAsync()
    {
        List<Taxis>? query = await _taxesFeesRepository.GetAllTaxisAsQueryable().Where(t => !t.IsDeleted ?? false).ToListAsync();

        return query
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

    public async Task<List<ItemSpecificTaxViewModel>> GetDefaultItemTaxesAsync(List<int> itemIds)
    {
        if (itemIds == null || !itemIds.Any())
            return new List<ItemSpecificTaxViewModel>();

        IQueryable<Item>? query = _itemRepository.GetAllItemsAsQueryable();

        List<ItemSpecificTaxViewModel>? itemSpecificTaxViewModels = await query.Where(i => itemIds.Contains(i.ItemId) && i.IsDefaultTax == true)
                 .Select(
                     i =>
                         new ItemSpecificTaxViewModel
                         {
                             ItemId = i.ItemId,
                             Percentage = i.TaxPercentage,
                             TaxName = TaxConstants.OTHER
                         }
                 )
                 .ToListAsync();


        return itemSpecificTaxViewModels;
    }

    #endregion
}
