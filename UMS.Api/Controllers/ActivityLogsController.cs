using Data;
using LoggerLibrary.Interface;
using LoggerLibrary.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;

namespace UMS.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class ActivityLogsController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IActivityLogsService m_activityLogsService;

        public ActivityLogsController(ILogService logger, IActivityLogsService activityLogsService) 
        {
            m_logService = logger;
            m_activityLogsService = activityLogsService;
        }

        //GET ACTIVITY LOGS

        [HttpGet("activity_logs/{page}/{page_size}")]
        public IActionResult GetActivityLogs(int platformId, int page, int page_size, int roleId, string firstDateString, string lastDateString, long? userId)
        {
            m_logService.Log("Activity Logs -> GetActivityLogs() was called");

            try 
            {
                //int parsedYear = int.Parse(year);
                //int parsedMonth = int.Parse(month);

                //int lastDayMonth;
                //int lastDayYear;

                //if (parsedMonth == 12)
                //{
                //    lastDayYear = parsedYear + 1;
                //    lastDayMonth = 1;
                //}
                //else
                //{
                //    lastDayYear = parsedYear;
                //    lastDayMonth = parsedMonth + 1;
                //}

                DateTime firstDay = DateTime.Parse(firstDateString);
                DateTime lastDay = DateTime.Parse(lastDateString);

                List<ActivityLogDTO> result = new List<ActivityLogDTO>();

                

                if (platformId != 0 && platformId != null) 
                { 
                    if(roleId != 0 && platformId != null) 
                    {
                        result = m_activityLogsService.getPlatformRoleActivityLogs(firstDay, lastDay, platformId, roleId, userId);
                    }
                    else
                    {
                        result = m_activityLogsService.getPlatformActivityLogs(firstDay, lastDay, platformId, userId);
                    }
                }
                else
                {
                    result = m_activityLogsService.getAllActivityLogs(firstDay, lastDay, userId);
                }

                //Pagination
                int offset = (page - 1) * page_size;
                page = page - 1;

                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.Skip(offset).Take(page_size).OrderBy(q => q.LogId).ToList();

                    var response = new
                    {
                        Response = result,
                        RowCount = rowCount,
                    };

                    m_logService.Log("User list: Get successful.");
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        Response = result,
                        RowCount = 0,
                    };

                    m_logService.Log($"User list: Not found.");
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        //POST ACTIVITY LOG

        [HttpPost("activity_logs/activity-log")]
        public IActionResult PostActivityLog(ActivityLogPostDTO activityLog)
        {
            m_logService.Log("Activity Logs -> PostActivityLog() was called");

            try 
            {
                m_activityLogsService.postActivityLog(activityLog);

                m_logService.Log("Post Activity Log: Post successful.");
                return Ok();
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
