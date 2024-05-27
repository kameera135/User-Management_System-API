using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.Options;

namespace SITCAMSClientIntegration.AuthorizationMiddleware
{
    /// <summary>
    /// Handler that handles the <see cref="RankRequirement"/> authorization
    /// </summary>
    public class RankAuthorizationHandler : AuthorizationHandler<RankRequirement>
    {
        private readonly CentralAuthOptions _centralAuthOptions;
        private readonly ILogger<RankAuthorizationHandler> _logger;
        
        public RankAuthorizationHandler(ILogger<RankAuthorizationHandler> logger, IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)
        {
            _centralAuthOptions = centralAuthOptions.Value;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RankRequirement requirement)
        {
            try
            {
                // The user name must be present
                if (_centralAuthOptions.UseUserName) { var tokenUserName = context.User.FindFirst("UserName").Value; }
                // The user ID must be present
                else { var tokenUserId = int.Parse(context.User.FindFirst("UserId").Value); }
                var tokenUserRank = int.Parse(context.User.FindFirst("UserRank").Value);

                if (tokenUserRank <= requirement.Rank)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }
    }

    /// <summary>
    /// Specifies that the class or method that this attribute is applied to
    /// requires a particular rank
    /// </summary>
    public class RequiredRankAttribute : AuthorizeAttribute
    {

        public RequiredRankAttribute(int rank) => Rank = rank;

        // Get or set the Age property by manipulating the underlying Policy property
        public int Rank
        {
            get
            {
                if (int.TryParse(Policy.Substring(AuthorizationPolicyProvider.REQUIRED_RANK_PREFIX.Length), out var rank))
                {
                    if (rank < 1) throw new ArgumentException($"Invalid rank of {rank}. Highest possible rank is 1");
                    return rank;
                }
                // The highest possible rank
                return 1;
            }
            set
            {
                Policy = $"{AuthorizationPolicyProvider.REQUIRED_RANK_PREFIX}{value}";
            }
        }
    }

    /// <summary>
    /// An authorization requirement based on permissions
    /// </summary>
    public class RankRequirement : IAuthorizationRequirement
    {
        public int Rank { get; private set; }

        public RankRequirement(int rank) { Rank = rank; }
    }
}