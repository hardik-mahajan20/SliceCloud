using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ModifierGroupRepository(SliceCloudContext sliceCloudContext) : IModifierGroupRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllModifierGroupsAsQueryable

    public IQueryable<ModifierGroup> GetAllModifierGroupsAsQueryable()
    {
        return _sliceCloudContext.ModifierGroups.AsQueryable();
    }

    #endregion

    #region GetModifierGroupById

    public async Task<ModifierGroup?> GetModifierGroupByIdAsync(int modifierGroupId)
    {
        return await _sliceCloudContext.ModifierGroups
            .FirstOrDefaultAsync(c => c.ModifierGroupId == modifierGroupId);
    }

    #endregion

    #region AddModifierGroup

    public async Task<int> AddModifierGroupAsync(ModifierGroup modifierGroup)
    {
        await _sliceCloudContext.ModifierGroups.AddAsync(modifierGroup);
        await _sliceCloudContext.SaveChangesAsync();
        return modifierGroup.ModifierGroupId;
    }

    #endregion

    #region UpdateModifierGroup

    public async Task<int> UpdateModifierGroupAsync(ModifierGroup modifierGroup)
    {
        _sliceCloudContext.ModifierGroups.Update(modifierGroup);
        await _sliceCloudContext.SaveChangesAsync();
        return modifierGroup.ModifierGroupId;
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
