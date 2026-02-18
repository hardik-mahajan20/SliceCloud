namespace SliceCloud.Repository.Models;

public partial class OrderedItem
{
    public int OrderedItemId { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public string? ItemWiseComment { get; set; }

    public int Quantity { get; set; }

    public int ReadyQuantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ServedAt { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderedItemModifier> OrderedItemModifiers { get; set; } = new List<OrderedItemModifier>();
}
