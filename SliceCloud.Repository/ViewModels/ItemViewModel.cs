using System.ComponentModel.DataAnnotations;

namespace SliceCloud.Repository.ViewModels;

public class ItemViewModel
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item Name is required.")]
    public string ItemName { get; set; } = null!;

    [Required(ErrorMessage = "Category is required.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Rate is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be a positive number.")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public int UnitId { get; set; }

    public bool IsAvailable { get; set; }

    [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100.")]
    public decimal? TaxPercentage { get; set; }

    [StringLength(10, ErrorMessage = "Shortcode cannot exceed 10 characters.")]
    public string? ShortCode { get; set; }

    public bool? IsFavorite { get; set; }

    public bool IsDefaultTax { get; set; }

    public string? ItemImg { get; set; }

    [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ModifierId { get; set; }

    [Required(ErrorMessage = "Item Type is required.")]
    public string? ItemType { get; set; }

    public IEnumerable<ModifierGroupViewModel>? ModifierGroups { get; set; }

    public IEnumerable<CategoryViewModel>? Categories { get; set; }

    public IEnumerable<UnitViewModel>? Units { get; set; }

    public List<KeyValuePair<int, string>>? ItemTypes { get; set; }
}
