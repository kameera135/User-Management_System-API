using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Services;

namespace UMS.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class PlatformConfigurationController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IConfigurationService m_configurationService;
        private readonly IActivityLogsService m_umsActivityLogService;
        private readonly UserRolePlatformService m_userRolePlatformService;

        public PlatformConfigurationController(ILogService logger, IConfigurationService configurationService, IActivityLogsService umsActivityLogService, UserRolePlatformService userRolePlatformService)
        {
            m_logService = logger;
            m_configurationService = configurationService;
            m_umsActivityLogService = umsActivityLogService;
            m_userRolePlatformService = userRolePlatformService;
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

        //GET PLATFORM LIST FOR COMBO BOX

        [HttpGet("configuration/platforms/combobox")]
        public IActionResult GetPlatformList()
        {
            m_logService.Log("View Users -> GetPlatformList() was called");

            try
            {
                List<ComboboxDTO> result = m_configurationService.getPlatformList();

                m_logService.Log("Platform list for combobox: Get successful.");
                return Ok(result);
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
                //m_umsActivityLogService.CreateLog("Get", "Get platform list combobox", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET PLATFORMS 

        [HttpGet("configuration/platforms/{page}/{page_size}")]
        public IActionResult GetPlatforms(int page, int page_size, string? searchedPlatformName)
        {
            m_logService.Log("Platform Configuration -> GetPlatforms() was called");

            try
            {

                List<PlatformDTO> result = new List<PlatformDTO>();

                if (searchedPlatformName == null)
                {
                    result = m_configurationService.getPlatforms();
                }
                else
                {
                    result = m_configurationService.searchedPlatforms(searchedPlatformName);
                }

                //Pagination
                int offset = (page - 1) * page_size;
                page = page - 1;

                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.Skip(offset).Take(page_size).OrderBy(q => q.PlatformId).ToList();

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
                //m_umsActivityLogService.CreateLog("Get", "Get platform list", extraData, 0, Int32.Parse(GetPlatformId().ToString()), null);
            }
        }

        //POST PLATFORM

        [HttpPost("configuration/platform")]
        public IActionResult PostPlatform(PlatformPostDTO platform, long createdBy)
        {
            m_logService.Log("Platform Configuration -> PostPlatform() was called");

            try
            {
                m_configurationService.postPlatform(platform, createdBy);

                m_logService.Log("New Platform: Created successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Plaform is created successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Create platform", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UPDATE PLATFORM

        [HttpPut("configuration/platform")]
        public IActionResult UpdatePlatform(PlatformPutDTO platform, long updatedBy)
        {
            m_logService.Log("Platform Configuration -> UpdatePlatform() was called");

            try
            {
                m_configurationService.updatePlatform(platform, updatedBy);

                m_logService.Log("Edit Platform: Updated successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Plaform is updated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Update platform details", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DELETE PLATFORMS

        [HttpDelete("configuration/platforms")]
        public IActionResult DeletePlatforms(List<long> platformList, long deletedBy)
        {
            m_logService.Log("Platform Configuration -> DeletePlatforms() was called");

            try
            {
                for (int i = 0; i < platformList.Count(); i++)
                {
                    m_configurationService.deletePlatform(platformList[i], deletedBy);
                }

                m_logService.Log("Delete Platforms: Deleted successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Plaforms are deleted successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Delete Platform", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }

        //GET PLATFORMS FOR SELECTED ROLE
        [HttpGet("configuration/platforms/roles/{roleId}")]
        public IActionResult GetPlatformsByRole(long roleId)
        {
            m_logService.Log("Roles Combo -> GetRoleListByPlatform() was called");

            try
            {
                List<PlatformsOfRoleDTO> result = m_configurationService.getPlatformsByRole(roleId);

                m_logService.Log("Role list for combobox using platform ID: Get successful.");
                return Ok(result);
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
               // m_umsActivityLogService.CreateLog("Get", "Get platform details for selected role", extraData,0, Int32.Parse(GetPlatformId().ToString()), null);
            }
        }

        //GET PLATFORM USERS
        [HttpGet("configuration/platforms/users/{page}/{page_size}")]
        public IActionResult GetPlatformUsers(int page, int page_size, string? searchedUserName, long platformId)
        {
            m_logService.Log("View Platform Users -> GetPlatformUsers() was called");

            try
            {
                List<PlatformUsersDTO> result = new List<PlatformUsersDTO>();

                if(searchedUserName == null)
                {
                    result = m_configurationService.getPlatformUsers(platformId);
                }
                else
                {
                    result = m_configurationService.searchedPlatformUsers(platformId,searchedUserName);
                }

                //Pagination
                int offset = (page - 1) * page_size;
                page = page - 1;

                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.Skip(offset).Take(page_size).OrderBy(q => q.UserId).ToList();

                    var response = new
                    {
                        Response = result,
                        RowCount = rowCount,
                    };

                    m_logService.Log("Platform user list: Get successful.");
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        Response = result,
                        RowCount = 0,
                    };

                    m_logService.Log($"Platform user list: Not found.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get users for respective platform", extraData, 0, Int32.Parse(GetPlatformId().ToString()), null);
            }
        }

        //ASSIGN USERS BULK TO PLATFORM
        [HttpPost("configuration/platform/assign_users")]
        public IActionResult AssignPlatformUsers(AssignPlatformUsersDTO users, long createdBy)
        {
            m_logService.Log("Assign Platform Users -> AssignUsers() was called");

            try
            {
                m_configurationService.assignPlatformUsers(users, createdBy);
                m_logService.Log("New PlatformUser : Created successfully");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "User bulk is assigned successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Assign bulk of users to a platform", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UNASSIGN USERS FROM PLATFORM
        [HttpDelete("configuration/platforms/users/unassign_users")]
        public IActionResult UnassignPlatformUsers(AssignPlatformUsersDTO users, long deletedBy)
        {
            m_logService.Log("Platform Configuration -> UnAssignPlatformUsers() was called");

            try
            {

                m_configurationService.unassignPlatformUsers(users, deletedBy);

                m_logService.Log("Unassign PlatformUsers: Unassign successfully.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Users unassigned from platform successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Un assign users from selected platform", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }
    }
}
