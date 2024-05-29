using Data;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Services
{
    public class UserRolePlatformService : IUserRolePlatformService
    {
        private readonly UmsContext m_umsContext;
        private readonly IConfiguration m_configuration;

        public UserRolePlatformService(UmsContext umsContext, IConfiguration configuration)
        {
            m_umsContext = umsContext;
            m_configuration = configuration;
        }

        public long GetPlatformId()
        {
            var platform = m_configuration.GetValue<long>("PlatformID");

            return platform;
        }

        public long GetRoleId(long userId)
        {
            var roleId = (from role in m_umsContext.Roles
                          join userRoles in m_umsContext.UserRoles on role.RoleId equals userRoles.RoleId
                          where (userRoles.UserId == userId && role.PlatformId == GetPlatformId())
                          select role.RoleId).FirstOrDefault();

            return roleId;
        }
    }
}