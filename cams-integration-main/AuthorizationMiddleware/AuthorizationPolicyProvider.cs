using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SITCAMSClientIntegration.AuthorizationMiddleware
{
    /// <summary>
    /// Provides an authorization policy with the name `Permission:name-of-permission`
    /// so that not all policies need to be registered in the servies
    /// </summary>
    public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        /// The prefix used for the permission policies
        public static string PERMISSION_PREFIX = "Permission:";
        /// The prefix used for the required rank policies
        public static string REQUIRED_RANK_PREFIX = "RequiredRank:";
        /// The prefix used for the rank verification policies
        public static string RANK_POLICY = "Rank";
        /// The prefix used for the role verification policies
        public static string ROLE_PREFIX = "Role:";
        

        /// The fallback policy provider
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // ASP.NET Core only uses one authorization policy provider, so if the custom implementation
            // doesn't handle all policies (including default policies, etc.) it should fall back to an
            // alternate provider.
            //
            // In this sample, a default authorization policy provider (constructed with options from the 
            // dependency injection container) is used if this custom provider isn't able to handle a given
            // policy name.
            //
            // If a custom policy provider is able to handle all expected policy names then, of course, this
            // fallback pattern is unnecessary.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.Equals(RANK_POLICY))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new VerifyRankRequirement());
                return Task.FromResult(policy.Build());
            }
            else if (policyName.StartsWith(PERMISSION_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                string permission = policyName.Substring(PERMISSION_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permission));
                return Task.FromResult(policy.Build());
            }
            else if (policyName.StartsWith(REQUIRED_RANK_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(policyName.Substring(REQUIRED_RANK_PREFIX.Length), out int rank))
                {
                    var policy = new AuthorizationPolicyBuilder();
                    policy.AddRequirements(new RankRequirement(rank));
                    return Task.FromResult(policy.Build());
                }
            }
            else if (policyName.StartsWith(ROLE_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                string role = policyName.Substring(ROLE_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new RoleRequirement(role));
                return Task.FromResult(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }
}