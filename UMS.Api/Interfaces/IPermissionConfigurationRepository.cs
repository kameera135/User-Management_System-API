using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface IPermissionConfigurationRepository
    {
        List<PermissionDTO> getPermissions(long platformId, string? searchedPermissionName);
        void postPermission(PermissionPostDTO permission, long createdBy);
        void updatePermission(PermissionPutDTO permission, long updatedBy);
        void deletePermission(long permissionId, long deletedBy);
        void activateDeactivatePermission(long permissionId, bool status, long updatedBy);
        List<RolesAndPlatformsGetDTO> getRolesAndPlatforms(long permissionId);
        List<GetPermissionDTO> getPermissionsForPlatforms(long platformId);
        List<PermissionsForPlatformRoleGetDTO> getPermissionsForPlatformRoles(long platformId, long roleId);
        List<GetPermissionsNotInRoleDTO> getPermissionsNotInRoles(long platformId, long roleId);
        void postPermissionsToRole(PostPermissionsToRoleDTO permission, long createdBy);
        void unassignPermissionFromRole(UnassignPermissionsDTO permisson, long deletedBy);
    }
}
