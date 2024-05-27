using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SITCAMSClientIntegration.Configurations;

namespace SITCAMSClientIntegration.Responses
{
    /// <summary>
    /// A response message containing a authentication token
    /// </summary>
    public class TokenResponseMessage : ResponseMessage
    {
        /// <summary>
        /// The authentication token
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// The refresh token
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The user Id of the current user
        /// </summary>
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        /// <summary>
        /// The unique username of the current user
        /// </summary>
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        /// <summary>
        /// The role of the current user
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }

        /// <summary>
        /// The email of the current user
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The rank of the current user
        /// </summary>
        [JsonPropertyName("rank")]
        [Range(1, 100)]
        public int Rank { get; set; }

        /// <summary>
        /// The permissions granted to the current user
        /// </summary>
        [JsonPropertyName("permissions")]
        public List<string> Permissions { get; set; }

        public static TokenResponseMessage Get(bool success, 
            string message, string token, string refreshToken, int userId, string username, string role, string email, List<string> permissions)
        {
            return new TokenResponseMessage
            {
                Success = success,
                Message = message,
                Token = token,
                RefreshToken = refreshToken,
                UserId = userId,
                UserName = username,
                Role = role,
                Email = email,
                Permissions = permissions
            };
        }
    }
}
