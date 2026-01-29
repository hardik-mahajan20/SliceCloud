using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IUsersLoginRepository _userLoginRepository;
    private readonly IRolesService _rolesService;

    public JwtService(IConfiguration configuration, IUsersLoginRepository userLoginRepository, IRolesService rolesService)
    {
        _key = configuration["Jwt:Key"]
                ?? throw new ArgumentNullException(nameof(configuration), "JWT key is missing in configuration.");

        _issuer = configuration["Jwt:Issuer"]
            ?? throw new ArgumentNullException(nameof(configuration), "JWT issuer is missing in configuration.");

        _audience = configuration["Jwt:Audience"]
            ?? throw new ArgumentNullException(nameof(configuration), "JWT audience is missing in configuration.");

        _userLoginRepository = userLoginRepository;
        _rolesService = rolesService;

    }

    #region GenerateJwtToken

    public async Task<string> GenerateJwtTokenAsync(string email, bool rememberMe = false)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.UTF8.GetBytes(_key);

        UsersLogin userDetail = await _userLoginRepository.GetUserLoginByEmailAsync(email)
               ?? throw new Exception("User not found while generating JWT token.");
        Role? role = await _rolesService.GetRoleByIdAsync(userDetail.RoleId)
        ?? throw new Exception("User role not found while generating JWT token.");

        int? userId = userDetail.UserId;
        if (userId is null || userId == 0)
        {
            throw new Exception("User id not found while generating JWT token.");
        }
        List<Claim>? claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role.RoleName),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (rememberMe)
        {
            claims.Add(new Claim("RememberMe", "True"));
        }

        SecurityTokenDescriptor? tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = rememberMe
                                ? DateTime.UtcNow.AddDays(7)
                                : DateTime.UtcNow.AddMinutes(15),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }

    #endregion

    #region ValidateToken

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
        byte[]? key = Encoding.UTF8.GetBytes(_key);
        try
        {
            TokenValidationParameters? validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    #endregion

}
