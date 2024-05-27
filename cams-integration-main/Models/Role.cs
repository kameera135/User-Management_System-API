using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SITCAMSClientIntegration.Configurations;

namespace SITCAMSClientIntegration.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, int.MaxValue)]
        public int Rank { get; set; }
        public List<Permission> Permissions { get; set; }
        
    }
}