namespace SliceCloud.Repository.ViewModels;

public class CustomerViewModel
{
    public int CustomerId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? CustomerName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal TotalOrder { get; set; }
}
