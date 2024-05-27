using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SITCAMSClientIntegration.DTOs
{
    public class PaginationDTO<T>
    {
        [JsonPropertyName("totalRecords")]
        public int TotalRecords { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("records")]
        public List<T> Records { get; set; }
    }
}
