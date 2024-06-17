using Data;
using System;
using System.Drawing.Text;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Services;

namespace UMS.Api.Repositories
{
    public class PermissionConfigurationRepository : IPermissionConfigurationRepository
    {
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;

        public PermissionConfigurationRepository(IRepository dbContext, UmsContext umsContext)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
        }

  

        public List<PermissionDTO> getPermissions(long platformId, string? searchedPermissionName)
        {
            try
            {
                List<PermissionDTO> permissions = new List<PermissionDTO>();

                if (searchedPermissionName != null) 
                {
                    permissions = ((from permission in m_umsContext.Permissions                            
                                          join platform in m_umsContext.Platforms on permission.PlatformId equals platform.PlatformId
                                          where (permission.DeletedAt == null && platform.DeletedAt == null && permission.PlatformId == platformId && permission.Permission1.ToLower().Contains(searchedPermissionName.ToLower()))
                                          select new PermissionDTO
                                          {
                                              PermissionId = permission.PermissionId,
                                              Permission = permission.Permission1,
                                              PlatformId = permission.PlatformId,
                                              PlatformName = platform.PlatformName,
                                              Status = permission.Status,
                                              CreatedAt = permission.CreatedAt,
                                          }).ToList());
                }
                else
                {
                    permissions = (from permission in m_umsContext.Permissions
                                    join platform in m_umsContext.Platforms on permission.PlatformId equals platform.PlatformId
                                    where (permission.DeletedAt == null && platform.DeletedAt == null && permission.PlatformId == platformId)
                                    select new PermissionDTO
                                    {
                                        PermissionId = permission.PermissionId,
                                        Permission = permission.Permission1,
                                        PlatformId = permission.PlatformId,
                                        PlatformName = platform.PlatformName,
                                        Status = permission.Status,
                                        CreatedAt = permission.CreatedAt,
                                    }).ToList();
                }

                return permissions;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void postPermission(PermissionPostDTO permission, long createdBy)
        {
            try
            {
                List<Permission> exsistingResult = m_dbContext.Get<Permission>().Where(q=> q.Permission1 == permission.Permission.ToLower().Replace(" ", "_") && q.DeletedAt==null && q.Status == true).ToList();

                var platform = m_dbContext.Get<Platform>().Where(p=>p.PlatformId == permission.PlatformId).ToList();

                if(platform.Count != 0)
                {
                    if (exsistingResult.Count() == 0)
                    {
                        Permission tempPermissionObj = new Permission
                        {
                            Permission1 = permission.Permission.ToLower().Replace(" ", "_"),
                            PlatformId = permission.PlatformId,
                            CreatedAt = DateTime.Now,
                            CreatedBy = createdBy,
                        };
                        m_dbContext.Create(tempPermissionObj);
                        m_dbContext.Save();
                    }
                    else
                    {
                        throw new Exception("Unable to create new permission. The specified permission name is already in use.");
                    }
                }
                else
                {
                    throw new Exception("Platform not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void updatePermission(PermissionPutDTO permission, long updatedBy)
        {
            try
            {
                List<Permission> exsistingResults = m_dbContext.Get<Permission>().Where(q => q.Permission1 == permission.Permission.ToLower().Replace(" ", "_") && q.DeletedAt == null && q.Status == true).ToList();

                Permission exsistingResult = m_dbContext.GetById<Permission>(permission.PermissionId);

                if (exsistingResults.Any(result => result.PermissionId != exsistingResult.PermissionId))
                {
                    throw new Exception("The specified permission name is already assigned to another permission. Please choose a unique name for the permission you are trying to update.");
                }
                else
                {
                    if (exsistingResult != null && exsistingResult.PermissionId == permission.PermissionId)
                    {
                        exsistingResult.Permission1 = permission.Permission.ToLower().Replace(" ", "_");
                        exsistingResult.UpdatedAt = DateTime.Now;
                        exsistingResult.UpdatedBy = updatedBy;

                        m_dbContext.Update(exsistingResult);
                        m_dbContext.Save();
                    }
                    else
                    {
                        throw new Exception("Permission Not found");    
                    }
                }      
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void deletePermission(long permissionId, long deletedBy)
        {
            try
            {
                Permission exsistingResult = m_dbContext.GetById<Permission>(permissionId);

                if (exsistingResult != null && exsistingResult.PermissionId == permissionId)
                {
                    exsistingResult.DeletedAt = DateTime.Now;
                    exsistingResult.DeletedBy = deletedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Permission Not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void activateDeactivatePermission(long permissionId, bool status ,long updatedBy)
        {
            try
            {
                Permission exsistingResult = m_dbContext.GetById<Permission>(permissionId);

                if (exsistingResult != null && exsistingResult.PermissionId == permissionId)
                {
                    exsistingResult.Status = status;
                    exsistingResult.UpdatedAt = DateTime.Now;
                    exsistingResult.UpdatedBy = updatedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Permission Not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RolesAndPlatformsGetDTO> getRolesAndPlatforms(long permissionId)
        {
            List<RolesAndPlatformsGetDTO> lstResult = new List<RolesAndPlatformsGetDTO>();

            var platformRoles = ((from permission in m_umsContext.Permissions
                      join rolePermissions in m_umsContext.RolePermissions on permission.PermissionId equals rolePermissions.PermissionId
                      join role in m_umsContext.Roles on rolePermissions.RoleId equals role.RoleId
                      join platform in m_umsContext.Platforms on role.PlatformId equals platform.PlatformId
                      where (permission.DeletedAt == null && permission.Status == true && rolePermissions.DeletedAt == null && role.DeletedAt == null && platform.DeletedAt == null && permission.PermissionId== permissionId)
                      select new RolesAndPlatformsGetDTO
                      {
                          Role = role.Role1,
                          Platform = platform.PlatformName,
                      }).ToList());

            var indipendantRoles = ((from permission in m_umsContext.Permissions
                      join rolePermissions in m_umsContext.RolePermissions on permission.PermissionId equals rolePermissions.PermissionId
                      join role in m_umsContext.Roles on rolePermissions.RoleId equals role.RoleId
                      where (permission.DeletedAt == null && permission.Status == true && rolePermissions.DeletedAt == null && role.DeletedAt == null && permission.PermissionId == permissionId && role.PlatformId==null)
                      select new RolesAndPlatformsGetDTO
                      {
                          Role = role.Role1,
                      }).ToList());

            var indipendantPlatforms = ((from permission in m_umsContext.Permissions
                      join platform in m_umsContext.Platforms on permission.PlatformId equals platform.PlatformId
                      where (permission.DeletedAt == null && permission.Status == true && platform.DeletedAt == null && permission.PermissionId == permissionId)
                      select new RolesAndPlatformsGetDTO
                      {
                          Platform = platform.PlatformName,
                      }).ToList());

            var indipendantplatformsToRemove = platformRoles.Select(x => x.Platform).ToHashSet();
            indipendantPlatforms = indipendantPlatforms.Where(x => !indipendantplatformsToRemove.Contains(x.Platform)).ToList();

            lstResult = platformRoles.Union(indipendantRoles).Union(indipendantPlatforms).ToList();
            return lstResult;
        }

        public List<GetPermissionDTO> getPermissionsForPlatforms(long platformId)
        {
            List<GetPermissionDTO> permissions = new List<GetPermissionDTO>();

            permissions = (from permission in m_umsContext.Permissions
                           where permission.DeletedAt == null && permission.Status == true && permission.PlatformId == platformId
                           select new GetPermissionDTO
                           {
                               permissionId = permission.PermissionId,
                               Permission = permission.Permission1,
                               platformId = platformId,

                           }).ToList();

            return permissions;

        }

        public List<PermissionsForPlatformRoleGetDTO> getPermissionsForPlatformRoles(long platformId, long roleId)
        {
            try
            {
                List<PermissionsForPlatformRoleGetDTO> permissions = new List<PermissionsForPlatformRoleGetDTO>();

                //check this logic again
                permissions = (from role in m_umsContext.Roles
                               join permission in m_umsContext.Permissions on role.PlatformId equals permission.PlatformId
                               join rolePermission in m_umsContext.RolePermissions on new { RoleId = role.RoleId, PermissionId = permission.PermissionId } equals new { RoleId = rolePermission.RoleId, PermissionId = rolePermission.PermissionId }
                               join platform in m_umsContext.Platforms on role.PlatformId equals platform.PlatformId
                               where (role.DeletedAt == null && role.Status == true && permission.DeletedAt == null && permission.Status == true && rolePermission.DeletedAt == null && role.RoleId == roleId && role.PlatformId == platformId)
                               select new PermissionsForPlatformRoleGetDTO
                               {
                                   PlatformId = permission.PlatformId,
                                   Platform = platform.PlatformName,
                                   PermissionId = permission.PermissionId,
                                   Permission = permission.Permission1


                               }).ToList();
                return permissions;
            }
            catch (Exception ex)
            {
                throw new Exception("Permissions not found");
            }
        }

        public List<GetPermissionsNotInRoleDTO> getPermissionsNotInRoles(long platformId, long roleId)
        {
            try
            {
                List<GetPermissionsNotInRoleDTO> permissions = new List<GetPermissionsNotInRoleDTO>();

                permissions = (from permission in m_umsContext.Permissions
                               where permission.DeletedAt == null && permission.Status ==true && permission.PlatformId == platformId && !m_umsContext.RolePermissions.Any(rp => rp.PermissionId == permission.PermissionId && rp.RoleId == roleId && rp.DeletedAt == null)
                               select new GetPermissionsNotInRoleDTO
                               {
                                   PermissionId = permission.PermissionId,
                                   Permission = permission.Permission1,

                               }).ToList();

                return (permissions); 

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public void postPermissionsToRole(PostPermissionsToRoleDTO permission, long createdBy)
        {
            try
            {
                Role? role = m_umsContext.Roles.Where(r=>r.RoleId == permission.RoleId && r.DeletedAt == null && r.Status == true).FirstOrDefault();

                if(role != null)
                {
                    foreach (long permissionId in permission.PermissionIds)
                    {
                        Permission? tempPermission = m_umsContext.Permissions.Where(p=>p.PermissionId == permissionId).FirstOrDefault();

                        if(tempPermission != null)
                        {
                            RolePermission? entryExist = m_umsContext.RolePermissions.FirstOrDefault(rp => rp.PermissionId.Equals(permissionId) && rp.RoleId.Equals(permission.RoleId));

                            if (entryExist == null)
                            {
                                RolePermission rolePermission = new RolePermission
                                {
                                    RoleId = permission.RoleId,
                                    PermissionId = permissionId,
                                    CreatedBy = createdBy,
                                    CreatedAt = DateTime.Now,
                                };

                                m_dbContext.Create(rolePermission);
                            }
                            else
                            {
                                entryExist.DeletedAt = null;
                                entryExist.DeletedBy = null;
                            }
                        }
                        else
                        {
                            throw new Exception("Permission not found");
                        }
                    }
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Role not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void unassignPermissionFromRole(UnassignPermissionsDTO permission, long deletedBy)
        {
            try
            {
               /* Role? role = m_umsContext.Roles.FirstOrDefault(r => r.RoleId == roleId);
                Permission? permission = m_umsContext.Permissions.FirstOrDefault(p=>p.PermissionId == permissionId);

                if (role != null)
                {
                    if(permission != null)
                    {
                        RolePermission? existingResult = m_umsContext.RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId && rp.RoleId == roleId);

                        if (existingResult != null)
                        {
                            existingResult.DeletedAt = DateTime.Now;
                            existingResult.DeletedBy = deletedBy;

                            m_dbContext.Update(existingResult);
                            m_dbContext.Save();
                        }
                        else
                        {
                            throw new Exception("Permission not assigned to given role");
                        }
                    }
                    else
                    {
                        throw new Exception("Permission not found");
                    }
                }
                else
                {
                    throw new Exception("Role not found");
                }*/

                if(permission.RoleId == 0)
                {
                    throw new Exception("Role Id not found");
                }

                Role? role = m_umsContext.Roles.FirstOrDefault(r=>r.RoleId == permission.RoleId);

                if(role == null)
                {
                    throw new Exception("Role is invalid. First create role first");
                }

                foreach (long permissionId in permission.PermissionIds)
                {
                    if (permissionId == 0)
                    {
                        throw new Exception("Permission Id not found");
                    }

                    RolePermission? rolePermissions = m_umsContext.RolePermissions.FirstOrDefault(rp=>rp.RoleId == permission.RoleId && rp.PermissionId == permissionId);

                    if (rolePermissions == null)
                    {
                        throw new Exception("Permissions are not assigned to roles");
                    }

                    rolePermissions.DeletedAt = DateTime.Now;
                    rolePermissions.DeletedBy = deletedBy;

                    m_dbContext.Update(rolePermissions);
                    m_dbContext.Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
