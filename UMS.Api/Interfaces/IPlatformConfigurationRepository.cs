using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface IPlatformConfigurationRepository
    {
        List<ComboboxDTO> getPlatformList();
        List<PlatformDTO> getPlatforms(string? searchedPlatformName);
        void postPlatform(PlatformPostDTO platform, long createdBy);
        void updatePlatform(PlatformPutDTO platform, long updatedBy);
        void deletePlatform(long platformId, long deletedBy);
        List<PlatformsOfRoleDTO> getPlatformsByRole(long roleId);
        List<PlatformUsersDTO> getPlatformUsers(long platformId, string? searchedUserName);
        void assignPlatformUsers(AssignPlatformUsersDTO users, long createdBy);
        void unassignPlatformUsers(AssignPlatformUsersDTO users, long deletedBy);
    }
}
