using System.ComponentModel.DataAnnotations;

namespace SITCAMSClientIntegration.Requests
{
    /// <summary>
    /// A token verification request
    /// </summary>
    public class AuthVerifyRequest
    {
        /// <summary>
        /// The token to be verified
        /// </summary>
        [Required]
        public string Token { get; set; }
        /// <summary>
        /// Behavior string (unused)
        /// </summary>
        public string BehaviorString { get; set; }
    }
}
