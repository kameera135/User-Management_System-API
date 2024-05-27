using System.Text.Json.Serialization;

namespace SITCAMSClientIntegration.Responses
{
    /// <summary>
    /// A response message to a token verification request
    /// </summary>
    public class TokenVerificationResponseMessage : ResponseMessage
    {
        /// <summary>
        /// The unique username of the current user
        /// </summary>
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        /// <summary>
        /// The unique userId of the current user
        /// </summary>
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        public static TokenVerificationResponseMessage Get(bool success, int userId, string message)
        {
            return new TokenVerificationResponseMessage { Success = success, UserId = userId, Message = message };
        }
        public static TokenVerificationResponseMessage Get(bool success, string username, string message)
        {
            return new TokenVerificationResponseMessage { Success = success, 
                UserName = username,
                Message = message };
        }
    }
}
