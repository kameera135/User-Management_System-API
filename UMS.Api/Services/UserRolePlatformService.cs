using Data;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Services
{
    public class UserRolePlatformService : IUserRolePlatformService
    {
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;

        public UserRolePlatformService(IRepository dbContext, UmsContext umsContext)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
        }


        public long GetPlatformId()
        {
            Platform? platform = m_umsContext.Platforms.FirstOrDefault(u => u.PlatformName.Replace(" ", "").ToLower() == "usermanagementsystem".ToLower());

            if(platform != null)
            {
                return platform?.PlatformId ?? 0;
            }
            else
            {
                throw new Exception("Platform not found");
            }

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
