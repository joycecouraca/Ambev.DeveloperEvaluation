using Ambev.DeveloperEvaluation.Common.Security;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class HttpContextUserProvider : IContextUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private CurrentUserClaims? _cachedUser = null;

    public HttpContextUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IUser GetCurrentUser()
    {
        if (_cachedUser != null)
        {
            return _cachedUser;
        }

        var userClaims = _httpContextAccessor.HttpContext?.User
                         ?? throw new InvalidOperationException("User must be logged in the system.");

        var userId = GetClaimValue(userClaims, ClaimTypes.NameIdentifier);
        var username = GetClaimValue(userClaims, ClaimTypes.Name);        
        var userRole = GetClaimValue(userClaims, ClaimTypes.Role);

        return _cachedUser ??= new CurrentUserClaims
        {
            Id = Guid.Parse(userId),
            Username = username,            
            Role = userRole
        };
    }

    private static string GetClaimValue(ClaimsPrincipal userClaims, string claimType)
    {
        return userClaims.FindFirst(claimType)?.Value
               ?? throw new InvalidOperationException($"Claim '{claimType}' is missing.");
    }

    private record CurrentUserClaims : IUser
    {
        public required Guid Id { get; init; }
        public required string Username { get; init; }        
        public required string Role { get; init; }        
    }
}