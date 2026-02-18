using SliceCloud.Repository.Enums;

namespace SliceCloud.Repository.ViewModels;

public class ModifierViewModel
{
    public int ModifierId { get; set; }

    public int ModifiedItemId { get; set; }

    public int OrderedItemId { get; set; }

    public int Orderid { get; set; }

    public int ItemModifierId { get; set; }

    public string ModifierName { get; set; } = null!;

    public int ModifierGroupId { get; set; }

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public int UnitId { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public ModifierType? ModifierType { get; set; }
}
