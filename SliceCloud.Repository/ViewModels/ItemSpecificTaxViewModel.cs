namespace SliceCloud.Repository.ViewModels;

public class ItemSpecificTaxViewModel
{
    public int ItemId { get; set; }

    public string TaxName { get; set; } = string.Empty;

    public decimal? Percentage { get; set; }
}
