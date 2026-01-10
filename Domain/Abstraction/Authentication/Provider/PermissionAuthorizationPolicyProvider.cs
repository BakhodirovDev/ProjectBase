using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Provider;

public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
        _options = options.Value;
    }

    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy != null)
        {
            return policy;
        }

        if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName.Substring("Permission:".Length);

            var policyBuilder = new AuthorizationPolicyBuilder();
            policyBuilder.AddRequirements(new PermissionRequirement(permission));

            return policyBuilder.Build();
        }

        return null;
    }
}