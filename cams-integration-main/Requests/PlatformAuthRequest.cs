using System.ComponentModel.DataAnnotations;

namespace SITCAMSClientIntegration.Requests
{
    /// <summary>
    /// A platform authentication request using username and password
    /// </summary>
    public class PlatformAuthRequest : AuthRequest
    {
        /// <summary>
        /// The current platform ID
        /// </summary>
        [Required]
        public int PlatformId { get; set; }
        /// <summary>
        /// The remote IP Address
        /// </summary>
        public string RemoteIPAddress { get; set; }
        /// <summary>
        /// The user agent
        /// </summary>
        public string UserAgent { get; set; }
    }
}
