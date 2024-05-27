using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Services;

namespace UMS.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class SystemTokensController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly ISystemTokensService m_systemTokensService;
        private readonly UserRolePlatformService m_userRolePlatformService;
        private readonly IActivityLogsService m_umsActivityLogService;


        public SystemTokensController(ILogService logger, ISystemTokensService systemTokensService, UserRolePlatformService userRolePlatformService, IActivityLogsService umsActivityLogService)
        {
            m_logService = logger;
            m_systemTokensService = systemTokensService;
            m_userRolePlatformService = userRolePlatformService;
            m_umsActivityLogService = umsActivityLogService;
        }

        object extraData = "";

        long GetPlatformId()
        {
            return m_userRolePlatformService.GetPlatformId();
        }

        long GetRoleId(long userId)
        {
            return m_userRolePlatformService.GetRoleId(userId);
        }

        //GET TOKEN
        [HttpGet("apiTokens/{page}/{page_size}")]
        public IActionResult GetTokens(int page, int page_size) {

            m_logService.Log("ApiTokens -> GetTokens() was called");

            try
            {

                List<TokenDTO> result = new List<TokenDTO>();

                result = m_systemTokensService.getTokens();

                //Pagination
                int offset = (page - 1) * page_size;
                page = page - 1;

                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.Skip(offset).Take(page_size).OrderBy(q => q.TokenId).ToList();

                    var response = new
                    {
                        Response = result,
                        RowCount = rowCount,
                    };

                    m_logService.Log("Platform list: Get successful.");
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        Response = result,
                        RowCount = 0,
                    };

                    m_logService.Log($"Platform list: Not found.");
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
            finally
            {
                //m_umsActivityLogService.CreateLog("Get", "Get system tokens", extraData,0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //POST TOKEN
        [HttpPost("token")]
        public IActionResult PostToken(TokenPostDTO token, long createdBy)
        {
            m_logService.Log("Token Configuration -> PostToken() was called");

            try
            {
                m_systemTokensService.postToken(token, createdBy);

                m_logService.Log("New Token: Created successful.");
                return Ok(new
                {
                    status = "Success",
                    message = "Token was created"
                });
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
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Create system token", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UPDATE TOKEN
        [HttpPut("token")]
        public IActionResult UpdateToken(TokenPutDTO token, long updatedBy)
        {
            m_logService.Log("Token Configuration -> UpdateToken() was called");

            try
            {
                m_systemTokensService.updateToken(token, updatedBy);

                m_logService.Log("Edit Platform: Updated successful.");
                return Ok(new
                {
                    status = "Success",
                    message = "Token was updated"
                });
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
            finally
            {
                m_umsActivityLogService.CreateLog("Put", "Update system token", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DELETE TOKEN
        [HttpDelete("tokens")]
        public IActionResult DeleteTokens(List<long> tokenList, long deletedBy)
        {
            m_logService.Log("Token Configuration -> DeleteTokens() was called");

            try
            {
                for (int i = 0; i < tokenList.Count(); i++)
                {
                    m_systemTokensService.deleteToken(tokenList[i], deletedBy);
                }

                m_logService.Log("Delete Tokens: Deleted successful.");
                return Ok(new
                {
                    status = "Success",
                    message = "Token was deleted"
                });
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
            finally
            {
                m_umsActivityLogService.CreateLog("Delete", "Delete system token", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }
    }
}
