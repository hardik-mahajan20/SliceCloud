namespace SliceCloud.Repository.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public int CategoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public int UnitId { get; set; }

    public bool? IsAvailable { get; set; }

    public decimal? TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public string ItemType { get; set; } = null!;

    public bool? IsFavorite { get; set; }

    public bool? IsDefaultTax { get; set; }

    public string? ItemImage { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ItemModifierGroupMap> ItemModifierGroupMaps { get; set; } = new List<ItemModifierGroupMap>();

    public virtual ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();

    public virtual Unit Unit { get; set; } = null!;
}
