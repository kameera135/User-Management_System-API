using System.Text.Json.Serialization;

namespace SITCAMSClientIntegration.DTOs
{
    public class RoleDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }
}
