namespace SliceCloud.Repository.ViewModels;

public class TaxViewModel
{
    public int TaxId { get; set; }

    public string TaxName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string TaxType { get; set; } = string.Empty;
}
