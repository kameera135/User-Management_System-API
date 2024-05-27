using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface IRoleConfigurationRepository
    {
        void activateDeactivateRole(long roleId, bool status, long updatedBy);
        void deleteRole(long roleId, long deletedBy);
        List<GetPermissionDTO> getPermissionsForRole(long roleId, long platformId);
        List<ComboboxDTO> getRoleListByPlatform(int platformId);
        List<RoleDTO> getRoles(long platformId,string? searchedRoleName);
        List<GetRolesForPlatformUsersDTO> getRolesforPlatformUsers(long userId, long platformId);
        void postRole(RolePostDTO role, long createdBy);
        void updateRole(RolePutDTO role, long updatedBy);
        void assignRolesToPlatforms(RolePostBulkDTO platform, long createdBy);
        List<GetRolesNotForUserDTO> getUnassignUserRoles(long userId, long platformId);
    }
}
