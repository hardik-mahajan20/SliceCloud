namespace SliceCloud.Repository.ViewModels;

public class OrderItemViewModel
{
    public string? ItemName { get; set; }

    public string? ItemWiseComment { get; set; }

    public int? ItemId { get; set; }

    public int? OrderId { get; set; }

    public int Quantity { get; set; }

    public int? ReadyQuantity { get; set; }

    public int OrderItemId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }

    public decimal ModifierTotal { get; set; }

    public List<ModifierViewModel> Modifiers { get; set; } = new List<ModifierViewModel>();
}
