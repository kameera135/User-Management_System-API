using System.Text.Json.Serialization;

namespace SITCAMSClientIntegration.DTOs
{
    public class EmployeeWithRoleDTO : EmployeeDTO
    {
        [JsonPropertyName("role")]
        public RoleDTO Role { get; set; }
    }
}
