using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class UnitService(IUnitRepository unitRepository) : IUnitService
{
    private readonly IUnitRepository _unitRepository = unitRepository;

    #region GetUnits

    public async Task<List<UnitViewModel>> GetUnitsAsync()
    {
        List<Unit>? units = await _unitRepository.GetAllUnitsAsQueryable().ToListAsync();

        return units.Select(u => new UnitViewModel
        {
            UnitId = u.UnitId,
            UnitName = u.UnitName,
            ShortName = u.ShortName
        }).ToList();
    }

    #endregion
}
