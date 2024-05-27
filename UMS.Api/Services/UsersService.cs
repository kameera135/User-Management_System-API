using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Repositories;

namespace UMS.Api.Services
{
    public class UsersService : IUserService
    {
        private readonly IUserRepository m_userRepository;

        public UsersService(IUserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public List<ComboboxDTO> getUserListForCombobox()
        {
            try
            {
                List<ComboboxDTO> tempUsers = m_userRepository.getUserList();
                return tempUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UserDTO> getAllUsers(string? userName)
        {
            try
            {
                List<UserDTO> users = m_userRepository.getUsers(0, userName);

                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UserDTO> getUsersByPlatformId(int platformId, string? userName)
        {
            try
            {
                List<UserDTO> users = m_userRepository.getUsers(platformId, userName);

                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void postUser(UserPostDTO user, long createdBy)
        {
            try
            {
                m_userRepository.postUser(user,createdBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void updateUser(UserPutDTO user, long updatedBy)
        {
            try
            {
                m_userRepository.updateUser(user,updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void deleteUser(long userId, long deletedBy)
        {
            try
            {
                m_userRepository.deleteUser(userId, deletedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UserPlatformsAndRolesDTO> getUserPlatformsAndRoles(long userId) 
        {
            try
            {
                List<UserPlatformsAndRolesDTO> platformsAndRoles = m_userRepository.getUserPlatformsAndRoles(userId);

                return platformsAndRoles;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public void postBulkUsers(List<UserPostDTO> users, List<BulkRoleAssignDTO> userRoles, long createdBy)
        {
            try
            {
                m_userRepository.postBulkUsers(users, userRoles, createdBy);
            }
            catch
            {
                throw;
            }
        }

        public List<PlatformUsersDTO> getUsersNotInPlatform(long platformId)
        {
            try
            {
                List<PlatformUsersDTO> users = m_userRepository.getUsersNotInPlatform(platformId, null);
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PlatformUsersDTO> searchedUsersNotInPlatform(long platformId, string? searchedUserName)
        {
            try
            {
                List<PlatformUsersDTO> users = m_userRepository.getUsersNotInPlatform(platformId, searchedUserName);
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UserRolesAndPermissionsDTO> getRolePermissions(long userId, long platformId)
        {
            try
            {
                List<UserRolesAndPermissionsDTO> users = m_userRepository.getRolePermissions(userId, platformId);
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void assignRoles(AssignRolesToUserDTO user, long createdBy)
        {
            try
            {
                m_userRepository.assignRoles(user, createdBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void unassignRoles(long userId, long roleId, long deletedBy)
        {
            try
            {
                m_userRepository.unassignRoles(userId,roleId, deletedBy);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public GetUserDTO getUserDetails(long userId)
        {
            try
            {
                GetUserDTO result = m_userRepository.getUserDetails(userId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
