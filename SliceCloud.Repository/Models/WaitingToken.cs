namespace SliceCloud.Repository.Models;

public partial class WaitingToken
{
    public int WaitingTokenId { get; set; }

    public int CustomerId { get; set; }

    public int NoOfPeople { get; set; }

    public int SectionId { get; set; }

    public bool? IsAssigned { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<WaitingTableMapping> WaitingTableMappings { get; set; } = new List<WaitingTableMapping>();
}
