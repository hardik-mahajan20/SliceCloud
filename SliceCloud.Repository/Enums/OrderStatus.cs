namespace SliceCloud.Repository.Enums;

/// <summary>
/// Represents orders's status for consistency across the entire project.
/// </summary>
public enum OrderStatus : int
{
    Pending = 0,
    InProgress = 1,
    Served = 2,
    Completed = 3,
    Cancelled = 4,
    OnHold = 5,
    Failed = 6
}
