using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Implementations;

namespace SliceCloud.Service.Attributes;

public class TableService(ITableRepository tableRepository) : ITableService
{
    ITableRepository _tableRepository = tableRepository;

    #region GetAllTables

    public async Task<List<TableViewModel>> GetAllTablesAsync()
    {
        List<Repository.Models.Table>? tables = await _tableRepository.GetAllTablesAsQueryable().Where(t => !t.IsDeleted ?? false).ToListAsync();

        return tables.Select(t => new TableViewModel
        {
            TableId = t.TableId,
            TableName = t.TableName,
            SectionId = t.SectionId,
            IsDeleted = t.IsDeleted
        }).ToList();
    }

    #endregion

    #region  GetPaginatedTablesBySectionId
    public async Task<PaginatedList<TableViewModel>> GetPaginatedTablesBySectionIdAsync(int sectionId, int pageNumber, int pageSize, string searchQuery)
    {
        IQueryable<Repository.Models.Table>? query = _tableRepository.GetAllTablesAsQueryable().Where(table => table.SectionId == sectionId && table.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            string trimmedSearch = searchQuery.Trim().ToLower();
            query = query.Where(
                table =>
                    table.TableName != null && table.TableName.ToLower().Contains(trimmedSearch)
            );
        }

        int totalCount = await query.CountAsync();
        List<Repository.Models.Table>? tables = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var mappedTables = tables.Select(table => new TableViewModel
        {
            TableId = table.TableId,
            TableName = table.TableName,
            Status = (TableStatus)(table.TableStatus ?? 0),
            Capacity = table.Capacity
        }).ToList();

        return new PaginatedList<TableViewModel>(mappedTables, totalCount, pageNumber, pageSize);
    }

    #endregion

    #region GetAllTableIds

    public async Task<List<int>> GetAllTableIdsAsync(int sectionId)
    {
        List<int>? tableIds = await _tableRepository.GetAllTablesAsQueryable().Where(table => table.IsDeleted == false && table.SectionId == sectionId)
                .Select(table => table.TableId)
                .ToListAsync();
        return tableIds;
    }

    #endregion
}
