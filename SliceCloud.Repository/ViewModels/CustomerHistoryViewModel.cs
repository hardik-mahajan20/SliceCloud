namespace SliceCloud.Repository.ViewModels;

public class CustomerHistoryViewModel
{
    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal MaxOrder { get; set; }

    public decimal AvgBill { get; set; }

    public DateTime ComingSince { get; set; }

    public int Visits { get; set; }

    public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
}
