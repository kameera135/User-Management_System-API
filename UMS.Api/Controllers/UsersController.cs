using Azure;
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
    public class UsersController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IUserService m_userService;
        private readonly IActivityLogsService m_umsActivityLogService;
        private readonly UmsContext m_umsContext;
        private readonly UserRolePlatformService m_userRolePlatformService;

        public UsersController(ILogService logger, IUserService userService, IActivityLogsService umsActivityLogService, UmsContext umsContext, UserRolePlatformService userRolePlatformService)
        {
            m_logService = logger;
            m_userService = userService;
            m_umsActivityLogService = umsActivityLogService;
            m_umsContext = umsContext;
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

        

        //GET USER LIST FOR COMBO BOX

        [HttpGet("users/combobox")]
        public IActionResult GetUserList()
        {
            m_logService.Log("View Users -> GetUserList() was called");

            try
            {
                List<ComboboxDTO> result = m_userService.getUserListForCombobox();

                m_logService.Log("User list for combobox: Get successful.");
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
               //m_umsActivityLogService.CreateLog("Get", "Get user list",extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //GET USERS

        [HttpGet("users/{page}/{page_size}")]
        public  IActionResult GetUsers(int platformId, int page, int page_size, string? userName)
        {
            m_logService.Log("View Users -> GetUsers() was called");


            try
            {
                List<UserDTO> result = new List<UserDTO>();

                if (platformId == 0)
                {
                    if (userName == null)
                    {
                        result = m_userService.getAllUsers(null);
                    }
                    else
                    {
                        result = m_userService.getAllUsers(userName);
                    }
                }
                else
                {
                    if (userName == null)
                    {
                        result = m_userService.getUsersByPlatformId(platformId, null);
                    }
                    else
                    {
                        result = m_userService.getUsersByPlatformId(platformId, userName);
                    }
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
            finally
            {
                //m_umsActivityLogService.CreateLog("Get", "Get users", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //POST USERS

        [HttpPost("user")]
        public IActionResult PostUser(UserPostDTO user, long createdBy)
        {
            m_logService.Log("View Users -> PostUser() was called");

            try
            {
                m_userService.postUser(user, createdBy);

                m_logService.Log("Add User: Created successful.");
                return Ok( new {
                    Status_code = 200,
                    Message = "User created successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Create user", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UPDATE USERS

        [HttpPut("user")]
        public IActionResult UpdateUser(UserPutDTO user, long updatedBy)
        {
            m_logService.Log("View Users -> UpdateUser() was called");


            try
            {
                m_userService.updateUser(user,updatedBy);

                m_logService.Log("Edit User: Updated successful.");
                return Ok(new
                {
                    status_code = 200,
                    message = "User updated successfully"
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
                m_umsActivityLogService.CreateLog("Put", "Update user details", extraData, Int32.Parse(updatedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(updatedBy).ToString()));
            }
        }

        //DELETE USERS

        [HttpDelete("users")]
        public IActionResult DeleteUsers(List<long> userIdList, long deletedBy)
        {
            m_logService.Log("View Users -> DeleteUsers() was called");


            try
            {
                for (int i = 0;i < userIdList.Count(); i++)
                {
                    m_userService.deleteUser(userIdList[i], deletedBy);
                }
              
                m_logService.Log("Delete Users: Deleted successful.");
                return Ok( new
                {
                    status_code = 200,
                    message = "User deleted successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Delete user details", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }


        //GET USER PLATFORMS AND ROLES
            
        [HttpGet("user/platforms_and_roles")]
        public IActionResult GetUserPlatformsAndRoles(long userId)
        {
            m_logService.Log("View Users -> GetUserPlatformsAndRoles() was called");

            try
            {
                List<UserPlatformsAndRolesDTO> result = m_userService.getUserPlatformsAndRoles(userId);

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
                //m_umsActivityLogService.CreateLog("Get", "Get user platform and role details", extraData, Int32.Parse(userId.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(userId).ToString()));
            }
        }

        //POST BULK USERS

        [HttpPost("users/bulk")]
        public IActionResult PostBulkUser(BulkPostDTO bulkData, long createdBy)
        {
            m_logService.Log("View Users -> PostBulkUser() was called");

            try
            {
                m_userService.postBulkUsers(bulkData.Users, bulkData.UserRoles, createdBy);

                m_logService.Log("Add Bulk Users: Created successful.");
                return Ok( new
                {
                    status_code = 200,
                    message = "user bulk created successfully"
                });
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
             

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred. Please check user/role lists and try again.");
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Create user bulk", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        // GET USERS WHO NOT IN SELECTED PLATFORM
        [HttpGet("users/platform/un-assigned")]
        public IActionResult GetUsersNotInPlatform(string? searchedUserName, long platformId)
        {
            m_logService.Log("View Users -> GetUsersNotInPlatform() was called");

            try
            {
                List<PlatformUsersDTO> result = new List<PlatformUsersDTO>();

                if (searchedUserName == null)
                {
                    result = m_userService.getUsersNotInPlatform(platformId);
                }
                else
                {
                    result = m_userService.searchedUsersNotInPlatform(platformId, searchedUserName);
                }


                if (result != null && result.Count > 0)
                {

                    var rowCount = result.Count();
                    result = result.OrderBy(q => q.UserId).ToList();

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
            finally
            {
                //m_umsActivityLogService.CreateLog("Get", "Get un assigned user details of respective platform", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }



        //GET USERS ROLES AND PERMISSIONS
        [HttpGet("users/roles_and_permissions")]
        public IActionResult GetRolePermissions(long userId, long platformId)
        {
            m_logService.Log("View Roles & Permissions -> GetRolePermissions() was called");

            try
            {
                List<UserRolesAndPermissionsDTO> result = m_userService.getRolePermissions(userId,platformId);
                m_logService.Log("Get User Roles and Permissions: Get successful.");
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
                //m_umsActivityLogService.CreateLog("Get", "Get user role and permission details of respective platforms", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }

        //ASSIGN ROLES TO USERS
        [HttpPost("user/roles")]
        public IActionResult AssignRoles(AssignRolesToUserDTO user, long createdBy)
        {
            m_logService.Log("Assign users -> AssignRoles() was called");

            try
            {
                m_userService.assignRoles(user, createdBy);
                m_logService.Log("Assign roles to user: post successful.");

                return Ok(new
                {
                    status_code = 200,
                    message = "Assign roles to user successfully"
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
                m_umsActivityLogService.CreateLog("Post", "Assign roles to user", extraData, Int32.Parse(createdBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(createdBy).ToString()));
            }
        }

        //UNASSIGN ROLES FROM USER
        [HttpDelete("user/roles")]
        public IActionResult UnassignRoles(long userId, long roleId, long deletedBy)
        {
            m_logService.Log("Unassign users -> UnassignRoles() was called");

            try
            {
                m_userService.unassignRoles(userId, roleId, deletedBy);
                m_logService.Log("Unssign roles to user: delete successful.");
                return Ok(new
                {
                    status_code = 200,
                    message = "Unassign roles from user successfully"
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
                m_umsActivityLogService.CreateLog("Delete", "Un assign roles from user", extraData, Int32.Parse(deletedBy.ToString()), Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(deletedBy).ToString()));
            }
        }


        //GET USER DETAILS FROM USER ID
        [HttpGet("user/userDetails")]
        public IActionResult GetUserDetails(long userId)
        {
            m_logService.Log("Get user details -> GetUserDetails() was called");

            try
            {

                GetUserDTO user = m_userService.getUserDetails(userId);
                m_logService.Log("Unssign roles to user: delete successful.");
                return Ok(user);

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
                //m_umsActivityLogService.CreateLog("Get", "Get selected user detailes", extraData, 0, Int32.Parse(GetPlatformId().ToString()), Int32.Parse(GetRoleId(0).ToString()));
            }
        }
    }
}
