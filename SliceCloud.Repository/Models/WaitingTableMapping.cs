namespace SliceCloud.Repository.Models;

public partial class WaitingTableMapping
{
    public int WaitingTableId { get; set; }

    public int WaitingTokenId { get; set; }

    public int TableId { get; set; }

    public virtual Table Table { get; set; } = null!;

    public virtual WaitingToken WaitingToken { get; set; } = null!;
}
