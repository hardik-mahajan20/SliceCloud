namespace SliceCloud.Service.Interfaces;

public interface ICurrentUserService
{
    /// <summary>
    /// Retrieves logged users Id.
    /// </summary>
    /// <returns>A logged users Id.</returns>
    int UserId { get; }

    /// <summary>
    /// Retrieves logged users UserName.
    /// </summary>
    /// <returns>A logged users UserName.</returns>
    string? UserName { get; }

    /// <summary>
    /// Retrieves logged users UserRole.
    /// </summary>
    /// <returns>A logged users UserRole.</returns>
    string? UserRole { get; }
}
