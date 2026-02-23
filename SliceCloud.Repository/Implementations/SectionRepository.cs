using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class SectionRepository(SliceCloudContext sliceCloudContext) : ISectionRepository
{
    private readonly SliceCloudContext _sliceCloudContext = sliceCloudContext;

    #region GetAllSectionsAsQueryable

    public IQueryable<Section> GetAllSectionsAsQueryable()
    {
        return _sliceCloudContext.Sections.AsQueryable();
    }

    #endregion

    #region GetSectionById

    public async Task<Section?> GetSectionByIdAsync(int sectionId)
    {
        return await _sliceCloudContext.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);
    }

    #endregion

    #region AddSection

    public async Task<bool> AddSectionAsync(Section section)
    {
        await _sliceCloudContext.Sections.AddAsync(section);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion

    #region UpdateSection

    public async Task<bool> UpdateSectionAsync(Section section)
    {
        _sliceCloudContext.Sections.Update(section);
        return await _sliceCloudContext.SaveChangesAsync() > 0;
    }

    #endregion
}
