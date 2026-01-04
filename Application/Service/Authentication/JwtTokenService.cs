using Consts;
using Domain.Abstraction.Jwt;
using Domain.EfClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Service;

public class JwtTokenService
{
    protected readonly JwtOption _jwtOptions;
    private Guid? _userId;
    protected string? _userName;
    protected bool? _isAuthenticated;
    protected readonly IUserRepository _userRepository;
    protected readonly ITokenRepository _tokenRepository;
    protected IHttpContextAccessor HttpContextAccessor { get; private set; }

    public JwtTokenService(IOptions<JwtOption> jwtOptions,
                           IHttpContextAccessor httpContextAccessor,
                           IUserRepository userRepository,
                           ITokenRepository tokenRepository)
    {
        _jwtOptions = jwtOptions.Value;
        HttpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
    }

    public virtual string? UserName
    {
        get
        {
            if (!IsAuthenticated)
            {
                return null;
            }

            if (_userName == null)
            {
                _userName = new JwtSecurityTokenHandler().ReadJwtToken(GetTokenStringFromRequest()).Claims.FirstOrDefault((Claim a) => a.Type == "sub")?.Value;
            }

            return _userName;
        }
    }

    public virtual bool IsAuthenticated
    {
        get
        {
            if (!_isAuthenticated.HasValue)
            {
                var tokenString = GetTokenStringFromRequest();

                if (string.IsNullOrWhiteSpace(tokenString))
                {
                    _isAuthenticated = false;
                }
                else
                {
                    _isAuthenticated = ValidateToken(tokenString).GetAwaiter().GetResult();
                }
            }

            return _isAuthenticated.Value;
        }
    }

    public virtual Guid? UserId
    {
        get
        {
            if (!IsAuthenticated)
            {
                return null;
            }

            if (!_userId.HasValue)
            {
                _userId = GetUserIdFromToken();
            }
            return _userId;
        }
    }

    public virtual User? User
    {
        get
        {
            if (!IsAuthenticated)
            {
                return null;
            }

            if (!UserId.HasValue)
            {
                return null;
            }

            var userResult = _userRepository.GetByIdAsync(UserId.Value).GetAwaiter().GetResult();

            if (userResult == null)
            {
                return null;
            }

            return userResult;
        }
    }

    public virtual bool IsSystemRole
    {
        get
        {
            if (!IsAuthenticated)
            {
                return false;
            }
            if (User == null)
            {
                return false;
            }
            return User.UserRoles.Any(role => role.Role.IsSystemRole);
        }
    }

    public virtual bool IsSystemAdmin
    {
        get
        {
            if (!IsAuthenticated)
            {
                return false;
            }
            if (User == null)
            {
                return false;
            }
            return User.UserRoles.Any(role => role.Role.IsSystemAdmin);
        }
    }

    public virtual Guid? AccessTokenId
    {
        get
        {
            if (!IsAuthenticated)
            {
                return null;
            }
            if (User == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadToken(GetTokenStringFromRequest()) as JwtSecurityToken;

            var refreshTokenIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "AccessTokenId")?.Value;

            if (string.IsNullOrEmpty(refreshTokenIdClaim)) return null;

            return Guid.Parse(refreshTokenIdClaim);
        }
    }

    public virtual bool ValidatePermissions(Enum[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            return false;

        if (!IsAuthenticated)
            return false;

        var user = User;
        if (user == null)
            return false;

        var userPermissions = user.UserRoles
            ?.SelectMany(role => role.Role?.RolePermissions?
                .Select(rolePermission => rolePermission.Permission?.Name?.ToString().ToLower())
                .Where(name => !string.IsNullOrEmpty(name)) ?? Enumerable.Empty<string>())
            .Distinct()
            .ToHashSet();

        if (userPermissions == null || !userPermissions.Any())
            return false;


        var permissionsToCheck = permissions.Select(GetFullPermissionName);

        return permissionsToCheck.All(permission => userPermissions.Contains(permission));
    }

    protected async Task<bool> ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
        };
        try
        {
            var principal = jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            var accessTokenIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "AccessTokenId")?.Value;

            if (string.IsNullOrEmpty(accessTokenIdClaim)) return false;

            var resultDb = await _tokenRepository.GetByAccessTokenId(Guid.Parse(accessTokenIdClaim));

            if (resultDb == null || !resultDb.IsSuccess || resultDb.Value == null)
                return false;

            bool isValid = resultDb.Value.StatusId != StatusIdConst.DELETED &&
                           principal.Identity?.IsAuthenticated == true &&
                           validatedToken.ValidTo > DateTime.UtcNow;

            return isValid;
        }
        catch
        {
            return false;
        }
    }

    protected async Task<bool> ValidateRefreshToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.RefreshTokenSecretKey))
        };
        try
        {
            var principal = jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            var refreshTokenIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "RefreshTokenId")?.Value;

            if (string.IsNullOrEmpty(refreshTokenIdClaim)) return false;

            var resultDb = await _tokenRepository.GetByRefreshTokenId(Guid.Parse(refreshTokenIdClaim));

            if (resultDb == null || !resultDb.IsSuccess || resultDb.Value == null)
                return false;

            bool isValid = resultDb.Value.StatusId != StatusIdConst.DELETED &&
                           principal.Identity?.IsAuthenticated == true &&
                           validatedToken.ValidTo > DateTime.UtcNow;

            return isValid;
        }
        catch
        {
            return false;
        }
    }

    protected virtual string? GetTokenStringFromRequest()
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
        {
            return null;
        }

        return authHeader.Substring("Bearer ".Length).Trim();
    }

    protected Guid? GetUserIdFromToken()
    {
        var tokenString = GetTokenStringFromRequest();

        if (string.IsNullOrEmpty(tokenString))
        {
            return null;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(tokenString) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return null;
            }

            var nameIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (nameIdClaim != null && Guid.TryParse(nameIdClaim.Value, out var parsedGuid))
            {
                return parsedGuid;
            }

            return null;
        }
        catch
        {
            return null;

        }
    }

    public static string GetFullPermissionName(Enum permission)
    {
        var enumType = permission.GetType();
        var enumTypeName = enumType.Name;

        // "Permissions" yoki "Permission" qismini olib tashlash
        var contextName = enumTypeName.Replace("Permissions", "")
                                       .Replace("Permission", "")
                                       .ToLowerInvariant();

        return $"{contextName}.{permission.ToString().ToLowerInvariant()}";
    }

    public virtual async Task<bool> IsAuthenticatedAsync()
    {
        if (_isAuthenticated.HasValue) return _isAuthenticated.Value;

        var tokenString = GetTokenStringFromRequest();
        if (string.IsNullOrWhiteSpace(tokenString))
        {
            _isAuthenticated = false;
            return false;
        }

        _isAuthenticated = await ValidateToken(tokenString);
        return _isAuthenticated.Value;
    }
}