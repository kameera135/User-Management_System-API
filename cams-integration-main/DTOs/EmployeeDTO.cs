using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SITCAMSClientIntegration.DTOs
{
    public class EmployeeDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("phoneNo")]
        public string PhoneNo { get; set; }

        [JsonPropertyName("emergencyContactNo")]
        public string EmergencyContactNo { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("identity")]
        public string Identity { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("socialStatus")]
        public string SocialStatus { get; set; }

        [JsonPropertyName("salary")]
        public float Salary { get; set; }

        [JsonPropertyName("employeeNo")]
        public string EmployeeNo { get; set; }

        [JsonPropertyName("joinedDate")]
        public DateTime JoinedDate { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("extraData")]
        public object ExtraData { get; set; }
    }
}
