using Domain.EfClasses.Authentication;
using Microsoft.AspNetCore.Authorization;
using Provider;
using System.Security.Claims;

namespace Handler;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly Serilog.ILogger _logger;

    public PermissionAuthorizationHandler(IPermissionService permissionService, Serilog.ILogger logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {

        if (context.User == null || !context.User.Identity?.IsAuthenticated == true)
        {
            return;
        }

        var permissionClaims = context.User.FindAll("permission").Select(c => c.Value).ToList();

        if (permissionClaims.Any(p => p == requirement.Permission))
        {
            context.Succeed(requirement);
            return;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return;
        }

        var permissionsResult = await _permissionService.GetByUserIdAsync(userId);
        if (!permissionsResult.IsSuccess)
        {
            return;
        }

        var userPermissions = permissionsResult.Data?.Select(p => p.Name).ToList() ?? new List<string>();

        var hasPermission = userPermissions.Any(p => p == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            _logger.Warning("=== PERMISSION CHECK END: FAILED (Permission not found) ===");
        }
    }
}