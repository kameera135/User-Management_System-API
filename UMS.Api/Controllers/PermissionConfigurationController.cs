using Azure;
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
    public class PermissionConfigurationController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IConfigurationService m_configurationService;
        private readonly UserRolePlatformService m_userRolePlatformService;
        private readonly IActivityLogsService m_umsActivityLogService;

        public PermissionConfigurationController(ILogService logger, IConfigurationService configurationService, UserRolePlatformService userRolePlatformService, IActivityLogsService umsActivityLogService)
        {
            m_logService = logger;
            m_configurationService = configurationService;
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

        //GET PERMISSIONS

        //[HttpGet("configuration/permissions/{page}/{page_size}"),Authorize(Roles = "Admin")]
        [HttpGet("configuration/permissions/{page}/{page_size}")]
        public IActionResult GetPermissions(int page, int page_size, string? searchedPermissionName, long platformId)
        {
            m_logService.Log("Permission Configuration -> GetPermission() was called");

            try
            {
                List<PermissionDTO> result = new List<PermissionDTO>();

                if (searchedPermissionName == null)
                {
                    result = m_configurationService.getPermissions(platformId);
                }
                else
                {
                    result = m_configurationService.searchedPermissions(searchedPermissionName, platformId);
                }

                //Pagination
                int offset = (page - 1) * page_size;
                page = page - 1;

                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.Skip(offset).Take(page_size).OrderBy(q => q.PermissionId).ToList();

                    var response = new
                    {
                        Response = result,
                        RowCount = rowCount,
                    };

                    m_logService.Log("Permission list: Get successful.");
                    return Ok(response);
                }
                else
                {
                    var response = new
                    {
                        Response = result,
                        RowCount = 0,
                    };

                    m_logService.Log($"Permission list: Not found.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get permission details", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //POST PERMISSION

        [HttpPost("configuration/permission")]
        public IActionResult PostPermission(PermissionPostDTO permission, long createdBy)
        {
            m_logService.Log("Permission Configuration -> PostPermission() was called");

            try
            {
                m_configurationService.postPermission(permission, createdBy);

                m_logService.Log("New Permission: Created successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Permissions is created successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Create permission", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UPDATE PERMISSION

        [HttpPut("configuration/permission")]
        public IActionResult UpdatePermission(PermissionPutDTO permission, long updatedBy)
        {
            m_logService.Log("Permission Configuration -> UpdatePermission() was called");

            try
            {
                m_configurationService.updatePermission(permission, updatedBy);

                m_logService.Log("Edit Permission: Updated successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Permission is updated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Update permission details", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DELETE PERMISSIONS

        [HttpDelete("configuration/permissions")]
        public IActionResult DeletePermissions(List<long> PermissionList, long deletedBy)
        {
            m_logService.Log("Permission Configuration -> DeletePermissions() was called");

            try
            {
                for (int i = 0; i < PermissionList.Count(); i++)
                {
                    m_configurationService.deletePermission(PermissionList[i], deletedBy);
                }

                m_logService.Log("Delete Permissions: Deleted successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Permissions are deleted successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Delete permission", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }

        //ACTIVATE PERMISSION

        [HttpPut("configuration/permissions/activate")]
        public IActionResult ActivatePermissions(List<long> PermissionList, long updatedBy)
        {
            m_logService.Log("Permission Configuration -> ActivatePermissions() was called");

            try
            {
                for (int i = 0; i < PermissionList.Count(); i++)
                {
                    m_configurationService.activatePermission(PermissionList[i], updatedBy);
                }

                m_logService.Log("Activate Permissions: Activated successful.");
                return Ok(new
                {
                    status_code = 200,
                    status = "Success",
                    message = "Permissions are activated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Activate permission", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DEACTIVATE PERMISSION

        [HttpPut("configuration/permissions/deactivate")]
        public IActionResult DeactivatePermissions(List<long> PermissionList, long updatedBy)
        {
            m_logService.Log("Permission Configuration -> DeactivatePermissions() was called");

            try
            {
                for (int i = 0; i < PermissionList.Count(); i++)
                {
                    m_configurationService.deactivatePermission(PermissionList[i], updatedBy);
                }

                m_logService.Log("Deactivate Permissions: Deactivated successful.");
                return Ok(new
                {
                    status_code = StatusCodes.Status200OK,
                    status = "Success",
                    message = "Permisssions are deactivated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Deactivate permission", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //GET PERMISSION ROLES & PLATFORMS

        [HttpGet("configuration/permission/platforms-roles")]
        public IActionResult GetRolesAndPlatforms(long permissionId)
        {
            m_logService.Log("Permission Configuration -> GetRolesAndPlatforms() was called");

            try
            {
                List<RolesAndPlatformsGetDTO> response = m_configurationService.getRolesAndPlatforms(permissionId);



                m_logService.Log("View platforms & roles: Get successful.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get permissions roles and platforms", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET PERMISIONS FOR PLATFORMS LIST
        [HttpPost("configuration/permissions/platforms")]
        public IActionResult GetPermissionsForPlatforms(List<long> platformsList)
        {
            try
            {
                List<GetPermissionDTO> response = new List<GetPermissionDTO>();

                m_logService.Log("Permission Configuration -> GetPermissionsForPlatforms() was called");

                for(int i = 0; i<platformsList.Count(); i++)
                {
                    response.AddRange(m_configurationService.getPermissionsForPlatforms(platformsList[i]));
                    //return Ok(response);
                }

                m_logService.Log("View Permissions: Get successful.");
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
               // m_umsActivityLogService.CreateLog("Post", "Get permissions for platform list", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET PERMISSION FOR PLATFORM AND ROLE
        [HttpGet("configuration/permissions/platforms/role")]
        public IActionResult GetPermissionsForPlatformRoles(long platformId, long roleId)
        {
            m_logService.Log("Permission Configuration -> GetPermissionsForPlatformRoles() was called");
            
            try
            {
                List<PermissionsForPlatformRoleGetDTO> response = m_configurationService.getPermissionsForPlatformRoles(platformId, roleId);
                m_logService.Log("View Permissions: Get successful.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get permisisons for platform and roles", extraData,0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET PERMISSIONS THAT NOT ASSIGN TO SELECETED ROLE
        [HttpGet("configuration/permissions/platform/not_in_role")]
        public IActionResult GetPermissionsNotInRoles(long platformId, long roleId)
        {
            m_logService.Log("Permission Configuration -> GetPermissionsNotInRoles() was called");

            try
            {
                List<GetPermissionsNotInRoleDTO> response = m_configurationService.getPermissionsNotInRoles(platformId, roleId);
                m_logService.Log("View Permissions: Get successful.");
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
               // m_umsActivityLogService.CreateLog("Get", "Get permissions that not assigned for selected role", extraData,0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //ASSIGN PERMISSIONS TO ROLE
        [HttpPost("configuration/permissions/role")]
        public IActionResult PostPermissionsToRole(PostPermissionsToRoleDTO permission, long createdBy)
        {
            m_logService.Log("Permission Configuration -> PostPermissionsToRole() was called");

            try
            {
                m_configurationService.postPermissionsToRole(permission, createdBy);
                return Ok(new
                {
                    status_code = StatusCodes.Status200OK,
                    status = "Success",
                    message = "Permissions assigned to role successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Assign permissions to roles", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UNASSIGN PERMISSIONS FROM ROLE
        [HttpDelete("configuration/permissions/role/unassign")]
        public IActionResult UnassignPermissionFromRole(long roleId, long permissionId, long deletedBy)
        {
            m_logService.Log("Permission Configuration -> UnassignPermissionsFromRole() was called");

            try
            {
                m_configurationService.unassignPermissionFromRole(roleId, permissionId, deletedBy);
                return Ok(new
                {
                    status_code = StatusCodes.Status200OK,
                    status = "Success",
                    message = "Permission unassigned successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Unassign permissions from role", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }
    }
}
