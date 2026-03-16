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

    public async Task<int> AddSectionAsync(Section section)
    {
        await _sliceCloudContext.Sections.AddAsync(section);
        await _sliceCloudContext.SaveChangesAsync();
        return section.SectionId;
    }

    #endregion

    #region UpdateSection

    public async Task<int> UpdateSectionAsync(Section section)
    {
        _sliceCloudContext.Sections.Update(section);
        await _sliceCloudContext.SaveChangesAsync();
        return section.SectionId;
    }

    #endregion

    #region SaveChanges

    public async Task<int> SaveChangesAsync()
    {
        return await _sliceCloudContext.SaveChangesAsync();
    }

    #endregion

}
