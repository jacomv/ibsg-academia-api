using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Academia.Infrastructure.Auth;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid Id
    {
        get
        {
            var sub = User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    public string Email =>
        User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User?.FindFirst(ClaimTypes.Email)?.Value
        ?? string.Empty;

    public UserRole Role
    {
        get
        {
            var role = User?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(role, out var parsed) ? parsed : UserRole.Student;
        }
    }
}
