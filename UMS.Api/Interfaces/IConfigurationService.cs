using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface IConfigurationService
    {
        List<ComboboxDTO> getPlatformList();
        List<PlatformDTO> getPlatforms();
        List<PlatformDTO> searchedPlatforms(string? searchedPlatformName);
        void postPlatform(PlatformPostDTO platform, long createdBy);
        void updatePlatform(PlatformPutDTO platform, long updatedBy);
        void deletePlatform(long platformId, long deletedBy);
        List<ComboboxDTO> getRoleListByPlatform(int platformId);
        List<RoleDTO> getRoles(string? searchedRoleName);
        List<PermissionDTO> searchedPermissions(string? searchedPermissionName, long platformId);
        List<PermissionDTO> getPermissions(long platformId);
        List<PlatformsOfRoleDTO> getPlatformsByRole(long roleId);
        void postPermission(PermissionPostDTO permission, long createdBy);
        void updatePermission(PermissionPutDTO permission, long updatedBy);
        void deleteRole(long roleId, long deletedBy);
        void postRole(RolePostDTO role, long createdBy);
        void updateRole(RolePutDTO role, long updatedBy);
        void activatePermission(long permissionId, long updatedBy);
        void deactivatePermission(long permissionId, long updatedBy);
        void activateRole(long roleId, long updatedBy);
        void deactivateRole(long roleId, long updatedBy);
        List<RolesAndPlatformsGetDTO> getRolesAndPlatforms(long permissionId);
        List<GetPermissionDTO> getPermissionsForRole(long roleId, long platformId);
        List<PlatformUsersDTO> getPlatformUsers(long platformId);
        List<PlatformUsersDTO> searchedPlatformUsers(long platformId, string searchedUserName);
        List<GetRolesForPlatformUsersDTO> getRolesforPlatformUsers(long userId, long platformId);
        List<GetPermissionDTO> getPermissionsForPlatforms(long platformId);
        void assignPlatformUsers(AssignPlatformUsersDTO users, long createdBy);
        void unassignPlatformUsers(AssignPlatformUsersDTO users, long deletedBy);
        void assignRolesToPlatforms(RolePostBulkDTO platform, long createdBy);
        List<PermissionsForPlatformRoleGetDTO> getPermissionsForPlatformRoles(long platformId, long roleId);
        List<GetPermissionsNotInRoleDTO> getPermissionsNotInRoles(long platformId, long roleId);
        void postPermissionsToRole(PostPermissionsToRoleDTO permission, long createdBy);
        void deletePermission(long permissionId, long deletedBy);
        List<GetRolesNotForUserDTO> getUnassignUserRoles(long userId, long platformId);
        List<RoleDTO> getRolesByPlatform(long platformId, string searchedRoleName);
        void unassignPermissionFromRole(UnassignPermissionsDTO permission, long deletedBy);
    }
}
