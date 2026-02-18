namespace SliceCloud.Repository.Models;

public partial class ItemModifierGroupMap
{
    public int ItemModifierGroupId { get; set; }

    public int ItemId { get; set; }

    public int ModifierGroupId { get; set; }

    public short? MinSelectionRequired { get; set; }

    public short? MaxSelectionAllowed { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual ModifierGroup ModifierGroup { get; set; } = null!;
}
