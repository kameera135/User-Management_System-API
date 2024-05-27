using Data;
using System.Security.Cryptography;
using System.Text;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Services;

namespace UMS.Api.Repositories
{
    public class ActivityLogsRepository: IActivityLogsRepository
    {
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;
        private readonly UserRolePlatformService m_userRolePlatformService;

        public ActivityLogsRepository(IRepository dbContext, UmsContext umsContext, UserRolePlatformService userRolePlatformService)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
            m_userRolePlatformService = userRolePlatformService;
        }

        //Get Platform Role Activity Logs
        public List<ActivityLogDTO> getPlatformRoleActivityLogs(DateTime firstDate, DateTime lastDate, int? platformId, int? roleId, long? userId)
        {
            try
            {
                List<ActivityLogDTO> lstResult = new List<ActivityLogDTO>();

                //Activity Logs of users who have been assigned to a role and the role has been assigned to a platform
                var activityLogs1 = ((from user in m_umsContext.Users
                                               join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                               join role in m_umsContext.Roles on activityLog.RoleId equals role.RoleId
                                               join platform in m_umsContext.Platforms on activityLog.PlatformId equals platform.PlatformId
                                               where firstDate<=activityLog.CreatedAt && activityLog.CreatedAt<lastDate && activityLog.PlatformId == platformId && activityLog.RoleId == roleId && activityLog.DeletedAt == null                                              
                                               select new ActivityLogDTO
                                               {
                                                   LogId = activityLog.LogId,
                                                   UserName = user.UserName,
                                                   UserId = user.UserId,
                                                   PlatformName = platform.PlatformName,
                                                   PlatformId = platform.PlatformId,
                                                   RoleName = role.Role1,
                                                   ActivityType = activityLog.ActivityType,
                                                   Description = activityLog.Description,
                                                   Details = activityLog.Details,
                                                   CreatedAt = activityLog.CreatedAt,
                                               }).ToList());

                var activityLogs3 = ((from user in m_umsContext.Users
                                      join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                      join platform in m_umsContext.Platforms on activityLog.PlatformId equals platform.PlatformId
                                      where firstDate <= activityLog.CreatedAt && activityLog.CreatedAt < lastDate && activityLog.PlatformId == platformId && activityLog.RoleId == null && activityLog.DeletedAt == null
                                      select new ActivityLogDTO
                                      {
                                          LogId = activityLog.LogId,
                                          UserName = user.UserName,
                                          UserId = user.UserId,
                                          PlatformName = platform.PlatformName,
                                          PlatformId = platform.PlatformId,
                                          ActivityType = activityLog.ActivityType,
                                          Description = activityLog.Description,
                                          Details = activityLog.Details,
                                          CreatedAt = activityLog.CreatedAt,
                                      }).ToList());

                var platformIdsToRemove = activityLogs1.Select(x => x.PlatformId).ToHashSet();
                activityLogs3 = activityLogs3.Where(x => !platformIdsToRemove.Contains(x.PlatformId)).ToList();

                lstResult = activityLogs1.Union(activityLogs3).ToList();

                return lstResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get Platform Activity Logs
        public List<ActivityLogDTO> getPlatformActivityLogs(DateTime firstDate, DateTime lastDate, int? platformId, long? userId)
        {
            try
            {
                List<ActivityLogDTO> lstResult = new List<ActivityLogDTO>();

                //Activity Logs of users who have been assigned to a role and the role has been assigned to a platform
                var activityLogs1 = ((from user in m_umsContext.Users
                                      join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                      join role in m_umsContext.Roles on activityLog.RoleId equals role.RoleId
                                      join platform in m_umsContext.Platforms on activityLog.PlatformId equals platform.PlatformId
                                      where firstDate <= activityLog.CreatedAt && activityLog.CreatedAt < lastDate && activityLog.PlatformId == platformId && activityLog.DeletedAt == null
                                      select new ActivityLogDTO
                                      {
                                          LogId = activityLog.LogId,
                                          UserName = user.UserName,
                                          UserId = user.UserId,
                                          PlatformName = platform.PlatformName,
                                          PlatformId = platform.PlatformId,
                                          RoleName = role.Role1,
                                          ActivityType = activityLog.ActivityType,
                                          Description = activityLog.Description,
                                          Details = activityLog.Details,
                                          CreatedAt = activityLog.CreatedAt,
                                      }).ToList());


                //Activity Logs of users who have not been assigned to a role but users have been assigned to a platform
                var activityLogs3 = ((from user in m_umsContext.Users
                                      join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                      join platform in m_umsContext.Platforms on activityLog.PlatformId equals platform.PlatformId
                                      where firstDate <= activityLog.CreatedAt && activityLog.CreatedAt < lastDate && activityLog.PlatformId == platformId && activityLog.RoleId == null && activityLog.DeletedAt == null
                                      select new ActivityLogDTO
                                      {
                                          LogId = activityLog.LogId,
                                          UserName = user.UserName,
                                          UserId = user.UserId,
                                          PlatformName = platform.PlatformName,
                                          PlatformId = platform.PlatformId,
                                          ActivityType = activityLog.ActivityType,
                                          Description = activityLog.Description,
                                          Details = activityLog.Details,
                                          CreatedAt = activityLog.CreatedAt,
                                      }).ToList());

                lstResult = activityLogs1.Union(activityLogs3).ToList();

                return lstResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get Platform Activity Logs
        public List<ActivityLogDTO> getAllActivityLogs(DateTime firstDate, DateTime lastDate, long? userId)
        {
            try
            {
                List<ActivityLogDTO> lstResult = new List<ActivityLogDTO>();

                var activityLogs1 = ((from user in m_umsContext.Users
                                      join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                      join role in m_umsContext.Roles on activityLog.RoleId equals role.RoleId
                                      join platform in m_umsContext.Platforms on activityLog.PlatformId equals platform.PlatformId
                                      where firstDate <= activityLog.CreatedAt && activityLog.CreatedAt < lastDate && activityLog.DeletedAt == null
                                      select new ActivityLogDTO
                                      {
                                          LogId = activityLog.LogId,
                                          UserName = user.UserName,
                                          UserId = user.UserId,
                                          PlatformName = platform.PlatformName,
                                          PlatformId = platform.PlatformId,
                                          RoleName = role.Role1,
                                          ActivityType = activityLog.ActivityType,
                                          Description = activityLog.Description,
                                          Details = activityLog.Details,
                                          CreatedAt = activityLog.CreatedAt,
                                      }).ToList());

                var activityLogs2 = ((from user in m_umsContext.Users
                                      join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                      join role in m_umsContext.Roles on activityLog.RoleId equals role.RoleId
                                      where firstDate <= activityLog.CreatedAt && activityLog.CreatedAt < lastDate && activityLog.PlatformId == null && activityLog.DeletedAt == null
                                      select new ActivityLogDTO
                                      {
                                          LogId = activityLog.LogId,
                                          UserName = user.UserName,
                                          UserId = user.UserId,
                                          RoleName = role.Role1,
                                          ActivityType = activityLog.ActivityType,
                                          Description = activityLog.Description,
                                          Details = activityLog.Details,
                                          CreatedAt = activityLog.CreatedAt,
                                      }).ToList());

                var activityLogs3 = ((from user in m_umsContext.Users
                                      join activityLog in m_umsContext.ActivityLogs on user.UserId equals activityLog.UserId
                                      join platform in m_umsContext.Platforms on activityLog.PlatformId equals platform.PlatformId
                                      where firstDate <= activityLog.CreatedAt && activityLog.CreatedAt < lastDate && activityLog.RoleId == null && activityLog.DeletedAt == null
                                      select new ActivityLogDTO
                                      {
                                          LogId = activityLog.LogId,
                                          UserName = user.UserName,
                                          UserId = user.UserId,
                                          PlatformName = platform.PlatformName,
                                          PlatformId = platform.PlatformId,
                                          ActivityType = activityLog.ActivityType,
                                          Description = activityLog.Description,
                                          Details = activityLog.Details,
                                          CreatedAt = activityLog.CreatedAt,
                                      }).ToList());


                lstResult = activityLogs1.Union(activityLogs3).Union(activityLogs2).ToList();

                return lstResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void postActivityLog(ActivityLogPostDTO activityLog)
        {
            try
            {

                ActivityLog tempActivityLogObj = new ActivityLog
                {
                    UserId = activityLog.UserId,
                    PlatformId = activityLog.PlatformId,
                    RoleId = activityLog.RoleId,
                    ActivityType = activityLog.ActivityType,
                    Description = activityLog.Description,
                    Details = activityLog.Details,
                    CreatedAt = DateTime.Now,
                    CreatedBy = activityLog.UserId,
                };

                m_dbContext.Create(tempActivityLogObj);
                m_dbContext.Save();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CreateLog(string type, string description, object extraData, int? userId, int? platformId, int? roleId)
        {
            try
            {
                var user = m_umsContext.Users.Where(p=>p.UserId == (long)userId).FirstOrDefault();
                if (user != null)
                {

                    ActivityLog tempActivityLogObj = new ActivityLog
                    {
                        UserId = (long)userId,
                        PlatformId = platformId,
                        RoleId = roleId,
                        ActivityType = type,
                        Description = description,
                        Details = extraData.ToString(), //has to change the data type of "Details" to 'object'
                        CreatedAt = DateTime.Now,
                        CreatedBy = userId,
                    };

                    m_umsContext.Add(tempActivityLogObj);
                    m_umsContext.SaveChanges();
                }

                else
                {
                    throw new Exception("User not found");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    
}
