using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface IActivityLogsService
    {
        List<ActivityLogDTO> getPlatformRoleActivityLogs(DateTime firstDate, DateTime lastDate, int platformId, int roleId, long? userId);
        List<ActivityLogDTO> getPlatformActivityLogs(DateTime firstDate, DateTime lastDate, int platformId, long? userId);
        List<ActivityLogDTO> getAllActivityLogs(DateTime firstDate, DateTime lastDate, long? userId);
        void postActivityLog(ActivityLogPostDTO activityLog);

        void CreateLog(string type, string description, object extraData, int userId, int? platformId, int? roleId);
    }
}
