using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class ModifierRepository(SliceCloudContext sliceCloudContext) : IModifierRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllModifiersAsync

    public IQueryable<Modifier> GetAllModifiersAsQueryable()
    {
        return _sliceCloudContext.Modifiers.AsQueryable();
    }

    #endregion

    #region AddModifier

    public async Task<int> AddModifierAsync(Modifier modifier)
    {
        await _sliceCloudContext.Modifiers.AddAsync(modifier);
        await _sliceCloudContext.SaveChangesAsync();
        return modifier.ModifierId;
    }

    #endregion
}
