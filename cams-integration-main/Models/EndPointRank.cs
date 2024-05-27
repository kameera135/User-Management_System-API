using System.ComponentModel.DataAnnotations;
using SITCAMSClientIntegration.Configurations;

namespace SITCAMSClientIntegration.Models
{
    public class EndPointRank
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [RegularExpression("^[^/].*", ErrorMessage = "Endpoint must not start with '/'")]
        public string EndPoint { get; set; }
        [Range(1, int.MaxValue)]
        public int Rank { get; set; } = RankConfig.DefaultRank;
    }
}