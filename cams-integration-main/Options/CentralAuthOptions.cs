using System.ComponentModel.DataAnnotations;

namespace SITCAMSClientIntegration.Options
{
    public class CentralAuthOptions
    {
        [Required]
        public int PlatformId { get; set; }
        [Required]
        public string BaseUrl { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Logout { get; set; }
        [Required]
        public string VerifyToken { get; set; }
        [Required]
        public string SingleAuthVerify { get; set; }
        [Required]
        public string VerifyPlatformToken { get; set; }
        [Required]
        public int SlidingExpiration { get; set; }
        [Required]
        public int AbsoluteExpiration { get; set; }
        [Required]
        public int PlatformSlidingExpiration { get; set; }
        [Required]
        public int PlatformAbsoluteExpiration { get; set; }
        [Required]
        public bool UseUserName { get; set; }
        [Required]
        public string CAMSTokenValue { get; set; }
        [Required]
        public string CreateActivityLogPath { get; set; }
        [Required]
        public string UserProfilePath { get; set; }
        [Required]
        public string GetAllUsersPath { get; set; }
    }
}
