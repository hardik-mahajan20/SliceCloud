using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SliceCloud.Repository.Constants;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int UserId
    {
        get
        {
            string? claimValue = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(claimValue, out int userId)
                ? userId
                : throw new UnauthorizedAccessException(ErrorConstants.USER_ID_CLAIM_MISSING);
        }
    }

    public string? UserName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }

}
