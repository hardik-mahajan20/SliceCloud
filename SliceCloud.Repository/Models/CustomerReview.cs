namespace SliceCloud.Repository.Models;

public partial class CustomerReview
{
    public int ReviewId { get; set; }

    public int OrderId { get; set; }

    public decimal? FoodRating { get; set; }

    public decimal? ServiceRating { get; set; }

    public decimal? AmbienceRating { get; set; }

    public float? AverageRating { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Order Order { get; set; } = null!;
}
