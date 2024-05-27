using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.MemoryCaches;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Repositories.Interfaces;

namespace SITCAMSClientIntegration.AuthorizationMiddleware
{
    /// <summary>
    /// Handler that handles the <see cref="VerifyRankRequirement"/> authorization
    /// </summary>
    public class VerifyRankAuthorizationHandler : AuthorizationHandler<VerifyRankRequirement>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MemoryCache _cache;
        private readonly CentralAuthOptions _centralAuthOptions;
        private readonly ILogger<VerifyRankAuthorizationHandler> _logger;

        public VerifyRankAuthorizationHandler(IUserRepository userRepository,
            HandlerMemoryCache cache,
            IOptionsSnapshot<CentralAuthOptions> centralAuthOptions,
            IHttpContextAccessor httpContextAccessor,
            ILogger<VerifyRankAuthorizationHandler> logger)
        {
            _userRepository = userRepository;
            _cache = cache.Cache;
            _centralAuthOptions = centralAuthOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VerifyRankRequirement requirement)
        {
            try
            {
                // The user ID must be present
                string apiEndpoint = _httpContextAccessor.HttpContext.Request.Path;
                // The user name must be present
                if (_centralAuthOptions.UseUserName) { var tokenUserName = context.User.FindFirst("UserName").Value; }
                // The user ID must be present
                else { var tokenUserId = int.Parse(context.User.FindFirst("UserId").Value); }
                var tokenUserRank = int.Parse(context.User.FindFirst("UserRank").Value);

                // Get the rank for this endpoint from the dataset, returns the default rank if endpoint not in database
                int requiredRank = _cache.GetOrCreate<int>(apiEndpoint,
                    entry =>
                    {
                        entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(_centralAuthOptions.AbsoluteExpiration));
                        entry.SetSlidingExpiration(TimeSpan.FromSeconds(_centralAuthOptions.SlidingExpiration));
                        return _userRepository.GetEndPointRank(apiEndpoint);
                    });



                if (tokenUserRank <= requiredRank)
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

    public class VerifyRankAttribute : AuthorizeAttribute
    {
        public VerifyRankAttribute()
        {
            Policy = AuthorizationPolicyProvider.RANK_POLICY;
        }
    }

    public class VerifyRankRequirement : IAuthorizationRequirement { }
}