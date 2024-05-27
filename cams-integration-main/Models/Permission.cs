using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SITCAMSClientIntegration.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Role> Roles { get; set; }
    }
}