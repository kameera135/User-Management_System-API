using System.ComponentModel.DataAnnotations;

namespace SITCAMSClientIntegration.Requests
{
    /// <summary>
    /// A central platform 
    /// </summary>
    public class CentralPlatformAuthVerifyRequest
    {
        /// <summary>
        /// The Id of the current platform
        /// </summary>
        [Required]
        public int PlatformId { get; set; }

        /// <summary>
        /// The token to be verified
        /// </summary>
        [Required]
        public string Token { get; set; }
    }
}
