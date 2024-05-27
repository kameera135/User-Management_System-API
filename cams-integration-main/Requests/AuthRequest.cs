using System.ComponentModel.DataAnnotations;

namespace SITCAMSClientIntegration.Requests
{
    /// <summary>
    /// An authentication request using username and password
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// The username
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// The password
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Behavior string (unused)
        /// </summary>
        public string BehaviorString { get; set; }
        /// <summary>
        /// The active directory ID. Null if Active Directory isn't used.
        /// 0 for the core to set the ID
        /// </summary>
        public int? ActiveDirectoryId { get; set; } = 0;
    }
}
