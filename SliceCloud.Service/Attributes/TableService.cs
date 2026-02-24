using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Enums;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Implementations;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Attributes;

public class TableService(ITableRepository tableRepository, ICurrentUserService currentUserService) : ITableService
{
    ITableRepository _tableRepository = tableRepository;
    ICurrentUserService _currentUserService = currentUserService;

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

    #region IsDuplicateTableName

    public async Task<bool> IsDuplicateTableNameAsync(string tableName, int sectionId, int? excludeTableId = null)
    {
        bool isDuplicate = await _tableRepository.GetAllTablesAsQueryable()
                                .AnyAsync(
                                    table => table.TableName.ToLower() == tableName.ToLower()
                                    && table.SectionId == sectionId
                                    && (excludeTableId == null || table.TableId != excludeTableId));

        return isDuplicate;
    }

    #endregion

    #region AddTable

    public async Task<bool> AddTableAsync(TableViewModel tableViewModel)
    {
        Repository.Models.Table? table = new()
        {
            SectionId = tableViewModel.SectionId,
            TableName = tableViewModel.TableName!,
            Capacity = tableViewModel.Capacity,
            TableStatus = (int?)tableViewModel.Status,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        return await _tableRepository.AddTableAsync(table);
    }

    #endregion

    #region GetTableById

    public async Task<Repository.Models.Table?> GetTableByIdAsync(int tableId)
    {
        return await _tableRepository.GetAllTablesAsQueryable()
                                .FirstOrDefaultAsync(table => table.TableId == tableId
                                                            && table.IsDeleted == false);
    }

    #endregion

    #region UpdateTable

    public async Task<bool> UpdateTableAsync(TableViewModel tableViewModel)
    {
        Repository.Models.Table? table = await _tableRepository.GetTableByIdAsync(tableViewModel.TableId);
        if (table == null) return false;

        table.TableName = tableViewModel.TableName ?? string.Empty;
        table.Capacity = tableViewModel.Capacity;
        table.TableStatus = (int?)tableViewModel.Status;
        table.ModifiedAt = DateTime.UtcNow;
        table.ModifiedBy = _currentUserService.UserId;

        return await _tableRepository.UpdateTableAsync(table);
    }

    #endregion

    #region DeleteTable

    public async Task<bool> DeleteTableAsync(int tableId)
    {
        Repository.Models.Table? table = await _tableRepository.GetTableByIdAsync(tableId);
        if (table == null) return false;

        table.IsDeleted = true;
        table.ModifiedAt = DateTime.UtcNow;
        table.ModifiedBy = _currentUserService.UserId;

        return await _tableRepository.UpdateTableAsync(table);
    }

    #endregion
}
