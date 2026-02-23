using System.ComponentModel.DataAnnotations;
using SliceCloud.Repository.Enums;

namespace SliceCloud.Repository.ViewModels;

public class TableViewModel
{
    public int TableId { get; set; }

    public int? OrderId { get; set; }

    public PaginatedList<TableViewModel>? TablesPaginated { get; set; }

    [Required(ErrorMessage = "Table name is required.")]
    [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9 ]{1,19}$", ErrorMessage = "Table name must start with a letter and be 2-20 characters long, using only letters, numbers, and spaces.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Table name must be between 2 and 20 characters.")]
    public string? TableName { get; set; } = null!;

    public TableStatus Status { get; set; }

    public int? SectionId { get; set; }

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; }

    public bool? IsDeleted { get; set; }

    public IEnumerable<SectionViewModel>? Sections { get; set; }

    public int? SelectedSectionId { get; set; }

    public decimal? TotoaAmount { get; set; }

    public string? SelectedSectionName { get; set; }

    public DateTime? OccupiedStartTime { get; set; }
}
