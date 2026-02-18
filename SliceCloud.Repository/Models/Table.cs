namespace SliceCloud.Repository.Models;

public partial class Table
{
    public int TableId { get; set; }

    public int? SectionId { get; set; }

    public string TableName { get; set; } = null!;

    public int? TableStatus { get; set; }

    public int Capacity { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    public virtual Section? Section { get; set; }

    public virtual ICollection<WaitingTableMapping> WaitingTableMappings { get; set; } = new List<WaitingTableMapping>();
}
