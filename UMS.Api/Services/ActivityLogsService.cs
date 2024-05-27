using Data;
using LoggerLibrary.Interface;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;

namespace UMS.Api.Services
{
    public class ActivityLogsService : IActivityLogsService
    {
        private readonly IActivityLogsRepository m_activityLogsRepository;

        public ActivityLogsService(IActivityLogsRepository activityLogsRepository)
        {
            m_activityLogsRepository = activityLogsRepository;
        }

        public List<ActivityLogDTO> getPlatformRoleActivityLogs(DateTime firstDate, DateTime lastDate, int platformId, int roleId, long? userId)
        {
            try
            {
                List<ActivityLogDTO> platformsAndRoles = new List<ActivityLogDTO>();

                if (userId != null && userId != 0)
                {
                    platformsAndRoles = m_activityLogsRepository.getPlatformRoleActivityLogs(firstDate, lastDate, platformId, roleId, userId).Where(q=> q.UserId== userId).ToList();
                }
                else
                {
                    platformsAndRoles = m_activityLogsRepository.getPlatformRoleActivityLogs(firstDate, lastDate, platformId, roleId, userId);
                }              

                return platformsAndRoles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ActivityLogDTO> getPlatformActivityLogs(DateTime firstDate, DateTime lastDate, int platformId, long? userId)
        {
            try
            {
                List<ActivityLogDTO> platformsAndRoles = new List<ActivityLogDTO>();

                if (userId != null && userId != 0)
                {
                    platformsAndRoles = m_activityLogsRepository.getPlatformActivityLogs(firstDate, lastDate, platformId, userId).Where(q => q.UserId == userId).ToList();
                }
                else
                {
                    platformsAndRoles = m_activityLogsRepository.getPlatformActivityLogs(firstDate, lastDate, platformId, userId);
                }

                 

                return platformsAndRoles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ActivityLogDTO> getAllActivityLogs(DateTime firstDate, DateTime lastDate, long? userId)
        {
            try
            {
                List<ActivityLogDTO> platformsAndRoles = new List<ActivityLogDTO>();

                if (userId != null && userId != 0)
                {
                    platformsAndRoles = m_activityLogsRepository.getAllActivityLogs(firstDate, lastDate, userId).Where(q => q.UserId == userId).ToList();
                }
                else
                {
                    platformsAndRoles = m_activityLogsRepository.getAllActivityLogs(firstDate, lastDate, userId);
                }

                return platformsAndRoles;
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
                m_activityLogsRepository.postActivityLog(activityLog);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CreateLog(string type, string description, object extraData, int userId, int? platformId, int? roleId)
        {
            try
            {
                m_activityLogsRepository.CreateLog(type, description, extraData, userId, platformId, roleId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
