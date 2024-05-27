
using System.Collections.Generic;
using SITCAMSClientIntegration.Models;

namespace SITCAMSClientIntegration.Repositories.Interfaces
{
    /// <summary>
    /// Interface for the user repository that handles all user information for the platform
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Verifies that the user has the required permissions
        /// </summary>
        /// <param name="id">User's UserId corresponding to the Id of the user on CAMS</param>
        /// <param name="role">The user's role</param>
        /// <param name="permission">The required permission</param>
        /// <returns></returns>
        public bool IsAbleTo(int id, string role, string permission);

        /// <summary>
        /// Verifies that the user has the required permissions
        /// </summary>
        /// <param name="userName">User's UserName corresponding to the username of the user on CAMS</param>
        /// <param name="role">The user's role</param>
        /// <param name="permission">The required permission</param>
        /// <returns></returns>
        public bool IsAbleTo(string userName, string role, string permission);

        /// <summary>
        /// Retrieves a user with it's role information
        /// </summary>
        /// <param name="userId">UserId of the user</param>
        /// <returns></returns>
        User GetUserWithRole(int userId);

        /// <summary>
        /// Retrieves a user with it's role information
        /// </summary>
        /// <param name="userName">UserName of the user</param>
        /// <returns></returns>
        User GetUserWithRole(string userName);

        /// <summary>
        /// Updates the context with the user information
        /// </summary>
        /// <param name="userId">The user's Id</param>
        /// <param name="userName">The user's unique user name</param>
        /// <param name="roleName">The string name of the user's role</param>
        /// <param name="updatedRank">The new rank of the user</param>
        /// <param name="permissions">The list of permissions that the user has</param>
        void UpdateContext(int userId, string userName, string roleName, int updatedRank, List<string> permissions);
        /// <summary>
        /// Get the rank associated with a particular endpoint. Partial endpoints are
        /// also supported, and overridden only if a more specific endpoint is configured.
        /// </summary>
        /// <param name="endpoint">the endpoint without a leading '/'.
        /// e.g.: 'api/test/endpoint'</param>
        /// <returns>The rank configured for that endpoint.</returns>
        int GetEndPointRank(string endpoint);
    }
}