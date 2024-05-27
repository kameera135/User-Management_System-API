using System.Text.Json.Serialization;

namespace SITCAMSClientIntegration.Responses
{
    /// <summary>
    /// Generic response message from the central authentication module
    /// </summary>
    public class ResponseMessage
    {
        /// <summary>
        /// Whether authentication was successful or not
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// The authentication message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public static ResponseMessage Get(bool success, string message)
        {
            return new ResponseMessage { Success = success, Message = message };
        }
    }
}
