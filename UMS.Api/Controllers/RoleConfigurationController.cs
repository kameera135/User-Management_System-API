using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Services;

namespace UMS.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class RoleConfigurationController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IConfigurationService m_configurationService;
        private readonly UserRolePlatformService m_userRolePlatformService;
        private readonly IActivityLogsService m_umsActivityLogService;

        public RoleConfigurationController(ILogService logger, IConfigurationService configurationService, UserRolePlatformService userRolePlatformService, IActivityLogsService activityLogsService)
        {
            m_logService = logger;
            m_configurationService = configurationService;
            m_userRolePlatformService = userRolePlatformService;
            m_umsActivityLogService = activityLogsService;
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



        //GET ROLE LIST BY PLATFORM FOR COMBO BOX

        [HttpGet("configuration/roles/combobox/{platformId}")]
        public IActionResult GetRoleListByPlatform(int platformId)
        {
            m_logService.Log("Roles Combo -> GetRoleListByPlatform() was called");

            try
            {
                List<ComboboxDTO> result = m_configurationService.getRoleListByPlatform(platformId);

                m_logService.Log("Role list for combobox: Get successful.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get list of roles to combobox", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET ROLES 

        [HttpGet("configuration/roles/{page}/{page_size}")]
        public IActionResult GetRoles(long platformId, int page, int page_size, string? searchedRoleName)
        {
            m_logService.Log("Role Configuration -> GetRoles() was called");

            try
            {
                List<RoleDTO> result = new List<RoleDTO>();

                if(platformId == 0)
                {
                    if (searchedRoleName == null)
                    {
                        result = m_configurationService.getRoles(null);
                    }
                    else
                    {
                        result = m_configurationService.getRoles(searchedRoleName);
                    }
                }
                else
                {
                    if (searchedRoleName == null)
                    {
                        result = m_configurationService.getRolesByPlatform(platformId,null);
                    }
                    else
                    {
                        result = m_configurationService.getRolesByPlatform(platformId, searchedRoleName);
                    }
                }

                //Pagination
                int offset = (page - 1) * page_size;
                page = page - 1;

                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.Skip(offset).Take(page_size).OrderBy(q => q.RoleId).ToList();

                    var response = new
                    {
                        Response = result,
                        RowCount = rowCount,
                    };

                    m_logService.Log("Role list: Get successful.");
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        Response = result,
                        RowCount = 0,
                    };

                    m_logService.Log($"Role list: Not found.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get role details", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //POST ROLES
     
        [HttpPost("configuration/role")]

        public IActionResult PostRole(RolePostDTO role, long createdBy)
        {
            m_logService.Log("Role Configuration -> PostRole() was called");

            try
            {
                m_configurationService.postRole(role, createdBy);

                m_logService.Log("New Role: Created successful.");
                return Ok( new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Role is created successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Create role", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UPDATE ROLE
        [HttpPut("configuration/role")]
        public IActionResult UpdateRole(RolePutDTO role, long updatedBy)
        {
            m_logService.Log("Role Configuration -> UpdateRole() was called");

            try
            {
                m_configurationService.updateRole(role, updatedBy);

                m_logService.Log("Edit Role: Updated successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Role is updated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Update role details", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DELETE ROLE
        [HttpDelete("configuration/roles")]
        public IActionResult DeleteRoles(List<long> RoleList, long deletedBy)
        {
            m_logService.Log("Role Configuration -> DeleteRoles() was called");

            try
            {
                for (int i = 0; i < RoleList.Count(); i++)
                {
                    m_configurationService.deleteRole(RoleList[i], deletedBy);
                }

                m_logService.Log("Delete Roles: Deleted successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Roles are deleted successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Delete role", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }

        //ACTIVATE ROLE
        [HttpPut("configuration/roles/activate")]
        public IActionResult ActivateRoles(List<long> RoleList, long updatedBy)
        {
            m_logService.Log("Role Configuration -> ActivateRoles() was called");

            try
            {
                for (int i = 0; i < RoleList.Count(); i++)
                {
                    m_configurationService.activateRole(RoleList[i], updatedBy);
                }

                m_logService.Log("Activate Roles: Activated successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Roles are activated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Activate role", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DEACTIVATE ROLE
        [HttpPut("configuration/roles/deactivate")]
        public IActionResult DeactivateRoless(List<long> RoleList, long updatedBy)
        {
            m_logService.Log("Role Configuration -> DeactivateRoles() was called");

            try
            {
                for (int i = 0; i < RoleList.Count(); i++)
                {
                    m_configurationService.deactivateRole(RoleList[i], updatedBy);
                }

                m_logService.Log("Deactivate Roles: Deactivated successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Roles are deactivated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Deactivate role", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //GET PERMISSIONS FOR SELECTED ROLE AND PLATFORM
        [HttpGet("configuration/roles/platform-permission/{roleId}/{platformId}")]
        public IActionResult GetPermissionsForRole(long roleId, long platformId) {

            m_logService.Log("Role Configuration -> GetPlatformsAndPermissions() was called");

            try
            {
                List<GetPermissionDTO> response = m_configurationService.getPermissionsForRole(roleId,platformId);



                m_logService.Log("View permissions: Get successful.");
                return Ok(response);
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
                //m_umsActivityLogService.CreateLog("Get", "Get permission for selected role and platform", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET ROLES FOR SELECETD USER AND PLATFORM
        [HttpGet("configuration/roles/platforms/users")]
        public IActionResult GetRolesforPlatformUsers(long userId, long platformId)
        {
            m_logService.Log("Role Configuration -> GetRolesforPlatformUsers() was called");

            try
            {
                List<GetRolesForPlatformUsersDTO> response = m_configurationService.getRolesforPlatformUsers(userId, platformId);
                m_logService.Log("View roless: Get successful.");
                return Ok(response);
            }
            catch(Exception ex)
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
                //m_umsActivityLogService.CreateLog("Get", "Get roles for selected role and platform", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //ASSIGN ROLES TO PLATFORM LIST
        [HttpPost("configuration/roles/platform_list")]
        public IActionResult AssignRolesToPlatforms(RolePostBulkDTO platform, long createdBy)
        {
            m_logService.Log("Role Configuration -> AssignRolesToPlatforms() was called");

            try
            {
                m_configurationService.assignRolesToPlatforms(platform, createdBy);
                m_logService.Log("New Role: Assign to platform successfully ");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Roles are assigned successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Assign role for platform list", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //GET UNASSIGN ROLES FOR USER
        [HttpGet("configuration/roles/platform/not_for_users")]
        public IActionResult GetUnassignUserRoles(long userId, long platformId)
        {
            m_logService.Log("Role Configuration -> GetUnassignUserRoles() was called");

            try
            {
                List<GetRolesNotForUserDTO> response = m_configurationService.getUnassignUserRoles(userId,platformId);
                m_logService.Log("View Roles: Get successful.");
                return Ok(response);
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
                //m_umsActivityLogService.CreateLog("Get", "Get unassign role for user", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

    }
}
