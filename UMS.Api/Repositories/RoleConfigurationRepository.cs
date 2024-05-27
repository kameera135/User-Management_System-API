using Data;
using System;
using System.Data;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Repositories
{
    public class RoleConfigurationRepository : IRoleConfigurationRepository
    {
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;

        public RoleConfigurationRepository(IRepository dbContext, UmsContext umsContext)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
        }

        public List<ComboboxDTO> getRoleListByPlatform(int platformId)
        {
            try
            {
                List<Role> roles = m_dbContext.Get<Role>().Where(q => q.DeletedAt == null && q.PlatformId == platformId).ToList();

                List<ComboboxDTO> lstResult = new List<ComboboxDTO>();

                foreach (Role role in roles)
                {
                    ComboboxDTO tempRole = new()
                    {
                        Id = role.RoleId,
                        Value = role.Role1,
                    };

                    lstResult.Add(tempRole);
                }


                return lstResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<RoleDTO> getRoles(long platformId, string? searchedRoleName)
        {
            try
            {
                List<RoleDTO> roles = new List<RoleDTO>();

                if (platformId != 0)
                {
                    var allRolesAccordingToPlatform = (from role in m_umsContext.Roles
                                                       join platform in m_umsContext.Platforms on role.PlatformId equals platform.PlatformId
                                                       where (role.DeletedAt == null && platform.DeletedAt == null && platform.PlatformId == platformId)
                                                       select new RoleDTO
                                                       {
                                                           RoleId = role.RoleId,
                                                           Role = role.Role1,
                                                           platformId = role.PlatformId,
                                                           platform = platform.PlatformName,
                                                           status = role.Status,
                                                           CreatedAt = role.CreatedAt,
                                                       }).ToList();

                    if (searchedRoleName != null)
                    {
                        roles = (List<RoleDTO>)allRolesAccordingToPlatform.Where(q => q.Role.ToLower().Contains(searchedRoleName.ToLower())).ToList();
                    }
                    else
                    {
                        roles = allRolesAccordingToPlatform;
                    }
                }
                else
                { 
                    List<(Role role, Platform platform)> allRoles = new List<(Role,Platform)>(); 


                    if (searchedRoleName != null)
                    {
                        //roles = allRoles.Where(q => q.Role.ToLower().Contains(searchedRoleName.ToLower()) || q.platform.ToLower().Contains(searchedRoleName.ToLower())).ToList();

                        var joinedData1 = m_dbContext.Get<Role>()
                                         .Join(
                                             m_dbContext.Get<Platform>(),
                                             role => role.PlatformId,
                                             platform => platform.PlatformId,
                                             (role, platform) => new { Role = role, Platform = platform })
                                         .Where(q => q.Role.DeletedAt == null && q.Platform.DeletedAt == null && (q.Role.Role1.ToLower().Contains(searchedRoleName.ToLower())))
                                         .ToList();

                        allRoles = joinedData1.Select(q => (role: q.Role, platform: q.Platform)).ToList();
                    }
                    else
                    {
                        var joinedData2 = m_dbContext.Get<Role>()
                                         .Join(
                                             m_dbContext.Get<Platform>(),
                                             role => role.PlatformId,
                                             platform => platform.PlatformId,
                                             (role, platform) => new { Role = role, Platform = platform })
                                         .Where(q => q.Role.DeletedAt == null && q.Platform.DeletedAt == null)
                                         .ToList();

                        allRoles = joinedData2.Select(q => (role: q.Role,platform: q.Platform)).ToList();
                    }

                    if(allRoles.Count>0 && allRoles != null)
                    {
                        foreach((Role role, Platform platform) in allRoles)
                        {
                            RoleDTO tempRole = new()
                            {
                                RoleId = role.RoleId,
                                Role = role.Role1,
                                platformId = role.PlatformId,
                                platform = platform.PlatformName,
                                status = role.Status,
                                CreatedAt = role.CreatedAt,
                            };
                            roles.Add(tempRole);
                        }
                    }
                }

                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void postRole(RolePostDTO role, long createdBy)
        {
            try
            {
                foreach (long platformID in role.PlatformIds)
                {
                    Platform? Platform = m_umsContext.Platforms.FirstOrDefault(p => p.PlatformId == platformID);

                    if (Platform != null)
                    {
                        List<Role> excistingResult = m_dbContext.Get<Role>().Where(r => r.Role1 == role.Role.ToLower().Replace(" ", "_") && r.DeletedAt == null && r.Status == true && r.PlatformId == platformID).ToList();

                        if (excistingResult.Count == 0)
                        {
                            Role tempRoleObj = new Role
                            {
                                Role1 = role.Role.ToLower().Replace(" ", "_"),
                                PlatformId = platformID,
                                CreatedAt = DateTime.Now,
                                CreatedBy = createdBy,
                            };
                            m_dbContext.Create(tempRoleObj);
                        }
                        else
                        {
                            throw new Exception("Unable to create new role. The specified role name is already in use for given platform.");
                        }
                    }
                    else
                    {
                        throw new Exception("Platform not found");
                    }
                }

                m_dbContext.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void updateRole(RolePutDTO role, long updatedBy)
        {
            try
            {
                Role exsistingResult = m_dbContext.GetById<Role>(role.RoleId);

                List<Role> tempRole = m_dbContext.Get<Role>().Where(r => r.Role1 == role.Role.ToLower().Replace(" ", "_") && r.DeletedAt == null && r.Status == true).ToList();

                if(exsistingResult != null)
                {
                    if (tempRole.Any(result => result.RoleId != exsistingResult.RoleId))
                    {
                        throw new Exception("The specified role name is already assigned to another role. Please choose a unique name for the role you are trying to update.");
                    }
                    else
                    {
                        if (exsistingResult != null && exsistingResult.RoleId == role.RoleId)
                        {
                            foreach (long platformId in role.PlatformIds)
                            {
                                Platform? platform = m_umsContext.Platforms.FirstOrDefault(p => p.PlatformId == platformId);

                                if(platform != null)
                                {
                                    //to check platform ID is already in there
                                    if (exsistingResult.PlatformId == platformId)
                                    {
                                        exsistingResult.Role1 = role.Role;
                                        exsistingResult.Status = role.Status;
                                        exsistingResult.UpdatedAt = DateTime.Now;
                                        exsistingResult.UpdatedBy = updatedBy;

                                        m_dbContext.Update(exsistingResult);
                                        m_dbContext.Save();
                                    }
                                    else
                                    {
                                        Role newRole = new Role
                                        {
                                            Role1 = role.Role.ToLower().Replace(" ", "_"),
                                            PlatformId = platformId,
                                            CreatedAt = DateTime.Now,
                                            CreatedBy = updatedBy,
                                        };
                                        m_dbContext.Create(newRole);
                                        m_dbContext.Save();
                                        //throw new Exception("Try again");
                                    }
                                }
                                else
                                {
                                    throw new Exception("One or more platforms can not find from the already created platforms.");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Role not found");
                        }
                    }
                }
                else { throw new Exception("Role not found"); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void deleteRole(long roleId, long deletedBy)
        {
            try
            {
                Role exsistingResult = m_dbContext.GetById<Role>(roleId);

                if (exsistingResult != null && exsistingResult.RoleId == roleId)
                {
                    exsistingResult.DeletedAt = DateTime.Now;
                    exsistingResult.DeletedBy = deletedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Role Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void activateDeactivateRole(long roleId, bool status, long updatedBy)
        {
            try
            {
                Role exsistingResult = m_dbContext.GetById<Role>(roleId);

                if (exsistingResult != null && exsistingResult.RoleId == roleId)
                {
                    exsistingResult.Status = status;
                    exsistingResult.UpdatedAt = DateTime.Now;
                    exsistingResult.UpdatedBy = updatedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Role Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GetPermissionDTO> getPermissionsForRole(long roleId, long platformId)
        {
            try
            {
                List<GetPermissionDTO> permissions = new List<GetPermissionDTO>();

                permissions = (from role in m_umsContext.Roles
                               join permission in m_umsContext.Permissions on role.PlatformId equals permission.PlatformId
                               join rolePermission in m_umsContext.RolePermissions on new { RoleId = role.RoleId, PermissionId = permission.PermissionId } equals new { RoleId = rolePermission.RoleId, PermissionId = rolePermission.PermissionId }
                               join platform in m_umsContext.Platforms on role.PlatformId equals platform.PlatformId
                               where (role.DeletedAt == null && role.Status == true && permission.DeletedAt == null && rolePermission.DeletedAt == null && role.RoleId == roleId && role.PlatformId == platformId)
                               select new GetPermissionDTO
                               {
                                   platformId = permission.PlatformId,
                                   Platform = platform.PlatformName,
                                   permissionId = permission.PermissionId,
                                   Permission = permission.Permission1
                                   

                               }).ToList();

                return permissions;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GetRolesForPlatformUsersDTO> getRolesforPlatformUsers(long userId, long platformId)
        {
            try
            {
                List<GetRolesForPlatformUsersDTO> roles = new List<GetRolesForPlatformUsersDTO>();

                roles = (from role in m_umsContext.Roles
                         join userRole in m_umsContext.UserRoles on role.RoleId equals userRole.RoleId
                         join user in m_umsContext.Users on userRole.UserId equals user.UserId
                         join platform in m_umsContext.Platforms on role.PlatformId equals platform.PlatformId
                         where (role.DeletedAt == null && role.Status == true && userRole.DeletedAt == null && platform.DeletedAt == null && user.UserId == userId && platform.PlatformId == platformId)
                         select new GetRolesForPlatformUsersDTO
                         {
                             RoleId = role.RoleId,
                             PlatformId = platform.PlatformId,
                             Role = role.Role1,
                             PlatformName = platform.PlatformName,

                         }).ToList();

                return roles;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void assignRolesToPlatforms(RolePostBulkDTO platforms, long createdBy)
        {
            try
            {
                foreach(long id in platforms.PlatformId)
                {
                    Platform? platform = m_umsContext.Platforms.FirstOrDefault(p=>p.PlatformId == id);
                    if (platform != null)
                    {
                        Role role = new Role
                        {
                            Role1 = platforms.Role,
                            PlatformId = id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = createdBy
                        };
                        m_dbContext.Create(role);
                    }
                    else
                    {
                        throw new Exception("One or more platforms can not find from the already created platforms.");
                    }
                }

                m_dbContext.Save();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GetRolesNotForUserDTO> getUnassignUserRoles(long userId, long platformId)
        {
            try
            {
                List<GetRolesNotForUserDTO> roles = new List<GetRolesNotForUserDTO>();

                roles = (from role in m_umsContext.Roles
                         where role.DeletedAt == null && role.Status == true && role.PlatformId == platformId && !m_umsContext.UserRoles.Any(ur => ur.RoleId == role.RoleId && ur.UserId == userId && ur.DeletedAt == null)
                         select new GetRolesNotForUserDTO
                         {
                             RoleId = role.RoleId,
                             Role = role.Role1

                         }).ToList();

                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

