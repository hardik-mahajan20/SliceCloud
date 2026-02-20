using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int UserId =>
        int.Parse(_httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    public string? UserName =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.Name)?.Value;
}
