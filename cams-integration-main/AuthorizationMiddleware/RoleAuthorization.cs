using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.Configurations;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Repositories.Interfaces;

namespace SITCAMSClientIntegration.AuthorizationMiddleware
{
    /// <summary>
    /// Handler that handles the <see cref="RoleRequirement"/> authorization
    /// </summary>
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly CentralAuthOptions _centralAuthOptions;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RoleAuthorizationHandler> _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userRepository">The user repository</param>
        /// <param name="logger">The application logger</param>
        /// <param name="centralAuthOptions">The central authentication service options</param>
        public RoleAuthorizationHandler(IUserRepository userRepository, ILogger<RoleAuthorizationHandler> logger, IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)
        {
            _centralAuthOptions = centralAuthOptions.Value;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            try
            {
                var tokenUserRole = context.User.FindFirst("UserRole").Value;
                if (_centralAuthOptions.UseUserName)
                {
                    var tokenUserName = context.User.FindFirst("UserName").Value;
                }
                else
                {
                    var tokenUserId = int.Parse(context.User.FindFirst("UserId").Value);
                }
                if (tokenUserRole == RoleConfig.SuperAdmin)
                {
                    context.Succeed(requirement);
                }
                else if (tokenUserRole.Equals(requirement.Role, StringComparison.OrdinalIgnoreCase))
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
    /// requires a particular permission
    /// </summary>
    public class RequiredRoleAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="role">The permission requrired as a string</param>
        public RequiredRoleAttribute(string role) => Role = role;

        /// Get or set the Age property by manipulating the underlying Policy property
        public string Role
        {
            get
            {
                return Policy.Substring(AuthorizationPolicyProvider.ROLE_PREFIX.Length);
            }
            set
            {
                Policy = $"{AuthorizationPolicyProvider.ROLE_PREFIX}{value}";
            }
        }
    }

    /// <summary>
    /// An authorization requirement based on permissions
    /// </summary>
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string Role { get; private set; }

        public RoleRequirement(string role) { Role = role; }
    }
}