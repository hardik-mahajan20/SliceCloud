using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class ModifierService(IModifierRepository modifierRepository) : IModifierService
{
    private readonly IModifierRepository _modifierRepository = modifierRepository;

    #region GetModifiersByGroupIds

    public async Task<List<Modifier>> GetModifiersByGroupIdsAsync(List<int> modifierGroupIds)
    {
        return await _modifierRepository.GetModifierGroupModifierMappingAsQueryable()
                    .Where(mgm => modifierGroupIds.Contains(mgm.ModifierGroupId))
                        .Select(mgm => mgm.Modifier)
                            .Distinct()
                                .Include(m => m.ModifierGroupModifierMappings)
                                    .ToListAsync();
    }

    #endregion
}
