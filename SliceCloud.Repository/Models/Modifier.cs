namespace SliceCloud.Repository.Models;

public partial class Modifier
{
    public int ModifierId { get; set; }

    public int? ModifierType { get; set; }

    public string ModifierName { get; set; } = null!;

    public int UnitId { get; set; }

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<ModifierGroupModifierMapping> ModifierGroupModifierMappings { get; set; } = new List<ModifierGroupModifierMapping>();

    public virtual ICollection<OrderedItemModifier> OrderedItemModifiers { get; set; } = new List<OrderedItemModifier>();

    public virtual Unit Unit { get; set; } = null!;
}
