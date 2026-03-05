using System.ComponentModel.DataAnnotations;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.ViewModels;

public class EditMenuItemViewModel
{
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Item Name is required.")]
    // [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]{2,19}$", ErrorMessage = "Item Name must start with a letter and be 3-20 characters long, using only letters, numbers, and spaces.")]
    // [StringLength(20, MinimumLength = 3, ErrorMessage = "Item Name must be between 3 and 20 characters.")]
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

    public bool IsDefaultTax { get; set; }

    [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100.")]
    public decimal? TaxPercentage { get; set; }

    [StringLength(10, ErrorMessage = "Shortcode cannot exceed 10 characters.")]
    public string? ShortCode { get; set; }

    [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
    // [RegularExpression(@"^[A-Z][A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
    public string? Description { get; set; }

    public string? ItemType { get; set; }

    public string? ItemImg { get; set; }

    public IEnumerable<CategoryViewModel>? Categories { get; set; }

    public IEnumerable<UnitViewModel>? Units { get; set; }

    public List<ModifierGroup>? ModifierGroups { get; set; } = new List<ModifierGroup>();

    public List<ItemModifierGroupMapViewModel>? ModifierGroupMappings { get; set; }
}
