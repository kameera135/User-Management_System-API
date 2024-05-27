using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.ExternalHelpers;
using SITCAMSClientIntegration.AuthenticationMiddleware;
using SITCAMSClientIntegration.AuthorizationMiddleware;
using SITCAMSClientIntegration.Contexts;
using SITCAMSClientIntegration.Controllers;
using SITCAMSClientIntegration.MemoryCaches;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Repositories;
using SITCAMSClientIntegration.Repositories.Interfaces;
using SITCAMSClientIntegration.ExternalHelpers.Interfaces;

namespace SITCAMSClientIntegration
{
    /// <summary>
    /// Static class to register the service collection extension method used by SITCAMSClientIntegration
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registers the required services for integration with CAMS
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration information</param>
        /// <returns></returns>
        public static IServiceCollection AddCAMSServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<HandlerMemoryCache>();
            services.AddHttpContextAccessor();
            services.AddControllers().AddApplicationPart(typeof(CentralAuthController).Assembly)
                .AddApplicationPart(typeof(PlatformInfoController).Assembly);
            services.Configure<CentralAuthOptions>(configuration.GetSection("CentralAuth"));

            services.AddHttpClient("CentralAuthController");
            services.AddHttpClient("CentralTokenAuthenticationHandler");
            services.AddHttpClient("CentralPlatformTokenAuthenticationHandler");
            services.AddAuthentication()
               .AddScheme<AuthenticationSchemeOptions, CentralTokenAuthenticationHandler>("Token", null)
               .AddScheme<AuthenticationSchemeOptions, CentralPlatformTokenAuthenticationHandler>("PlatformToken", null);

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

            // As always, handlers must be provided for the requirements of the authorization policies
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, RankAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, VerifyRankAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

            // ExternalHelpers
            services.AddScoped<ISystemLogger, SystemLogger>();
            services.AddScoped<IUserInfo, UserInfo>();

            

            if (configuration.GetValue<bool>("TestSupport"))
            {
                Guid dbName = Guid.NewGuid();
                services.AddDbContext<CAMSDBContext>(options => options.UseSqlite("Data Source=" + dbName.ToString() + ".db"));
            }
            else
            {
                services.AddDbContext<CAMSDBContext>(
                    options =>
                    {
                        options.UseSqlServer(configuration.GetConnectionString("CAMSDBConnection"));
                        // options.EnableSensitiveDataLogging(true);
                    }
                );
            }
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}