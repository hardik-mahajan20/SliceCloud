namespace SliceCloud.Repository.Models;

public partial class OrderedItemModifier
{
    public int ModifiedItemId { get; set; }

    public int OrderedItemId { get; set; }

    public int Quantity { get; set; }

    public int OrderId { get; set; }

    public int ItemModifierId { get; set; }

    public virtual Modifier ItemModifier { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual OrderedItem OrderedItem { get; set; } = null!;
}
