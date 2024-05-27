using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface IUserService
    {
        List<UserDTO> getAllUsers(string? userName);
        List<UserDTO> getUsersByPlatformId(int platformId, string? userName);
        void postUser(UserPostDTO user, long createdBy);
        void updateUser(UserPutDTO user, long updatedBy);
        void deleteUser(long userId, long deletedBy);
        List<UserPlatformsAndRolesDTO> getUserPlatformsAndRoles(long userId);
        List<ComboboxDTO> getUserListForCombobox();
        void postBulkUsers(List<UserPostDTO> users, List<BulkRoleAssignDTO> userRoles, long createdBy);
        List<PlatformUsersDTO> searchedUsersNotInPlatform(long platformId, string searchedUserName);
        List<PlatformUsersDTO> getUsersNotInPlatform(long platformId);
        List<UserRolesAndPermissionsDTO> getRolePermissions(long userId, long platformId);
        void assignRoles(AssignRolesToUserDTO user, long createdBy);
        void unassignRoles(long userId, long roleId, long deletedBy);
        GetUserDTO getUserDetails(long userId);
    }
}
