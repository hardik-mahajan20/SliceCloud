namespace SliceCloud.Repository.Enums;

/// <summary>
/// Represents table's status for consistency across the entire project.
/// </summary>
public enum TableStatus : int
{
    Available = 1,
    Occupied = 2,
    Reserved = 3,
    Running = 4
}
