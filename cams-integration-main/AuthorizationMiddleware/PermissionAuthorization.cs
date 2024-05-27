using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Repositories.Interfaces;

namespace SITCAMSClientIntegration.AuthorizationMiddleware
{
    /// <summary>
    /// Handler that handles the <see cref="PermissionRequirement"/> authorization
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly CentralAuthOptions _centralAuthOptions;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userRepository">The user repository</param>
        /// <param name="logger">The application logger</param>
        /// <param name="centralAuthOptions">The central authentication service options</param>
        public PermissionAuthorizationHandler(IUserRepository userRepository, ILogger<PermissionAuthorizationHandler> logger, IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)
        {
            _centralAuthOptions = centralAuthOptions.Value;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            try
            {
                var tokenUserRole = context.User.FindFirst("UserRole").Value;
                if (_centralAuthOptions.UseUserName)
                {
                    var tokenUserName = context.User.FindFirst("UserName").Value;

                    if (_userRepository.IsAbleTo(tokenUserName, tokenUserRole, requirement.Permission))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
                else
                {
                    var tokenUserId = int.Parse(context.User.FindFirst("UserId").Value);

                    if (_userRepository.IsAbleTo(tokenUserId, tokenUserRole, requirement.Permission))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
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
    public class RequiredPermissionAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="permission">The permission requrired as a string</param>
        public RequiredPermissionAttribute(string permission) => Permission = permission;

        /// Get or set the Age property by manipulating the underlying Policy property
        public string Permission
        {
            get
            {
                return Policy.Substring(AuthorizationPolicyProvider.PERMISSION_PREFIX.Length);
            }
            set
            {
                Policy = $"{AuthorizationPolicyProvider.PERMISSION_PREFIX}{value}";
            }
        }
    }

    /// <summary>
    /// An authorization requirement based on permissions
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }

        public PermissionRequirement(string permission) { Permission = permission; }
    }
}