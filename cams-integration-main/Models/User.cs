using System.ComponentModel.DataAnnotations;
using SITCAMSClientIntegration.Configurations;

namespace SITCAMSClientIntegration.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}