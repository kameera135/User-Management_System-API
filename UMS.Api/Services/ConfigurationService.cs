using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Repositories;

namespace UMS.Api.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IPlatformConfigurationRepository m_platformConfigurationRepository;
        private readonly IRoleConfigurationRepository m_roleConfigurationRepository;
        private readonly IPermissionConfigurationRepository m_permissionConfigurationRepository;

        public ConfigurationService(IPlatformConfigurationRepository platformConfigurationRepository, IRoleConfigurationRepository roleConfigurationRepository, IPermissionConfigurationRepository permissionConfigurationRepository)
        {
            m_platformConfigurationRepository = platformConfigurationRepository;
            m_roleConfigurationRepository = roleConfigurationRepository;
            m_permissionConfigurationRepository = permissionConfigurationRepository;
        }

        //Platforms

        public List<ComboboxDTO> getPlatformList()
        {
            try
            {
                List<ComboboxDTO> temPlatforms = m_platformConfigurationRepository.getPlatformList();
                return temPlatforms;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PlatformDTO> getPlatforms()
        {
            try
            {
                List<PlatformDTO> temPlatforms = m_platformConfigurationRepository.getPlatforms(null);
                return temPlatforms;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PlatformDTO> searchedPlatforms(string? searchedPlatformName)
        {
            try
            {
                List<PlatformDTO> temPlatforms = m_platformConfigurationRepository.getPlatforms(searchedPlatformName);
                return temPlatforms;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void postPlatform(PlatformPostDTO platform, long createdBy)
        {
            try
            {
                m_platformConfigurationRepository.postPlatform(platform, createdBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void updatePlatform(PlatformPutDTO platform, long updatedBy)
        {
            try
            {
                m_platformConfigurationRepository.updatePlatform(platform, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void deletePlatform(long platformId, long deletedBy)
        {
            try
            {
                m_platformConfigurationRepository.deletePlatform(platformId, deletedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PlatformsOfRoleDTO> getPlatformsByRole(long roleId)
        {
            try
            {
                List<PlatformsOfRoleDTO> tempPlatforms = m_platformConfigurationRepository.getPlatformsByRole(roleId);
                return tempPlatforms;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public List<PlatformUsersDTO> getPlatformUsers(long platformId)
        {
            try
            {
                List<PlatformUsersDTO> platformUsers = m_platformConfigurationRepository.getPlatformUsers(platformId,null);
                return platformUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PlatformUsersDTO> searchedPlatformUsers(long platformId, string? searchedUserName ) {
            try
            {
                List<PlatformUsersDTO> platformUsers = m_platformConfigurationRepository.getPlatformUsers(platformId, searchedUserName);
                return platformUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void assignPlatformUsers(AssignPlatformUsersDTO users, long createdBy)
        {
            try
            {
                m_platformConfigurationRepository.assignPlatformUsers(users, createdBy);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void unassignPlatformUsers(AssignPlatformUsersDTO users, long deletedBy)
        {
            try
            {
                m_platformConfigurationRepository.unassignPlatformUsers(users, deletedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Roles

        public List<ComboboxDTO> getRoleListByPlatform(int platformId)
        {
            try
            {
                List<ComboboxDTO> tempRoles = m_roleConfigurationRepository.getRoleListByPlatform(platformId);
                return tempRoles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RoleDTO> getRoles(string? searchedRoleName)
        {
            try
            {
                List<RoleDTO> tempRole = m_roleConfigurationRepository.getRoles(0,searchedRoleName);
                return tempRole;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RoleDTO> getRolesByPlatform(long platformId, string? searchedRoleName)
        {
            try
            {
                List<RoleDTO> roles = m_roleConfigurationRepository.getRoles(platformId, searchedRoleName);
                return roles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        public void postRole(RolePostDTO role, long createdBy)
        {
            try
            {
                m_roleConfigurationRepository.postRole(role, createdBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void updateRole(RolePutDTO role, long updatedBy)
        {
            try
            {
                m_roleConfigurationRepository.updateRole(role, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void deleteRole(long roleId, long deletedBy)
        {
            try
            {
                m_roleConfigurationRepository.deleteRole(roleId, deletedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void activateRole(long roleId, long updatedBy)
        {
            try
            {
                m_roleConfigurationRepository.activateDeactivateRole(roleId, true, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void deactivateRole(long roleId, long updatedBy)
        {
            try
            {
                m_roleConfigurationRepository.activateDeactivateRole(roleId, false, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<GetPermissionDTO> getPermissionsForRole(long roleId, long platformId)
        {
            try
            {
                List<GetPermissionDTO> tempPlatformsPermissions = m_roleConfigurationRepository.getPermissionsForRole(roleId,platformId);
                return tempPlatformsPermissions;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<GetRolesForPlatformUsersDTO> getRolesforPlatformUsers(long userId, long platformId)
        {
            try
            {
                List<GetRolesForPlatformUsersDTO> temproles = m_roleConfigurationRepository.getRolesforPlatformUsers(userId, platformId);
                return temproles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void assignRolesToPlatforms(RolePostBulkDTO platform, long createdBy)
        {
            try
            {
                m_roleConfigurationRepository.assignRolesToPlatforms(platform, createdBy);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<GetRolesNotForUserDTO> getUnassignUserRoles(long userId, long platformId)
        {
            try
            {
                List<GetRolesNotForUserDTO> tempRole = m_roleConfigurationRepository.getUnassignUserRoles(userId,platformId);
                return tempRole;
            }
            catch(Exception ex)
            {
                throw;
            }
        }



        //Permissions
        public List<PermissionDTO> getPermissions(long platformId)
        {
            try
            {
                List<PermissionDTO> tempPermission = m_permissionConfigurationRepository.getPermissions(platformId, null);
                return tempPermission;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PermissionDTO> searchedPermissions(string? searchedPermissionName, long platformId)
        {
            try
            {
                List<PermissionDTO> tempPermissions = m_permissionConfigurationRepository.getPermissions(platformId, searchedPermissionName);
                return tempPermissions;
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
                m_permissionConfigurationRepository.postPermission(permission, createdBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void updatePermission(PermissionPutDTO permission, long updatedBy)
        {
            try
            {
                m_permissionConfigurationRepository.updatePermission(permission, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void deletePermission(long permissionId, long deletedBy)
        {
            try
            {
                m_permissionConfigurationRepository.deletePermission(permissionId, deletedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void activatePermission(long permissionId, long updatedBy)
        {
            try
            {
                m_permissionConfigurationRepository.activateDeactivatePermission(permissionId, true, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void deactivatePermission(long permissionId, long updatedBy)
        {
            try
            {
                m_permissionConfigurationRepository.activateDeactivatePermission(permissionId, false, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RolesAndPlatformsGetDTO> getRolesAndPlatforms(long permissionId)
        {
            try
            {
                List<RolesAndPlatformsGetDTO> tempRolesPlatforms = m_permissionConfigurationRepository.getRolesAndPlatforms(permissionId);
                return tempRolesPlatforms;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<GetPermissionDTO> getPermissionsForPlatforms(long platformId)
        {
            try
            {
                List<GetPermissionDTO> tempPermission = m_permissionConfigurationRepository.getPermissionsForPlatforms(platformId);
                return tempPermission;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PermissionsForPlatformRoleGetDTO> getPermissionsForPlatformRoles(long platformId, long roleId)
        {
            try
            {
                List<PermissionsForPlatformRoleGetDTO> tempPermissions = m_permissionConfigurationRepository.getPermissionsForPlatformRoles(platformId, roleId);
                return tempPermissions;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<GetPermissionsNotInRoleDTO> getPermissionsNotInRoles(long platformId, long roleId)
        {
            try
            {
                List<GetPermissionsNotInRoleDTO> tempPermission = m_permissionConfigurationRepository.getPermissionsNotInRoles(platformId, roleId);
                return tempPermission;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void postPermissionsToRole(PostPermissionsToRoleDTO permission, long createdBy)
        {
            try
            {
                m_permissionConfigurationRepository.postPermissionsToRole(permission, createdBy);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void unassignPermissionFromRole(UnassignPermissionsDTO permission, long deletedBy)
        {
            try
            {
                m_permissionConfigurationRepository.unassignPermissionFromRole(permission, deletedBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
   
}
