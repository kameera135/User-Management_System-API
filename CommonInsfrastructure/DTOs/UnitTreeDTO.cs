using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonInsfrastructure.DTOs
{
    internal class UnitTreeDTO
    {
        public string UnitId { get; set; } = null!;
    }
    public class AssertTreeEntityDTO
    {
        public string name { get; set; }
        public string id { get; set; }
        public string icon { get; set; }
        public bool isUnit { get; set; }
        public List<AssertTreeEntityDTO> children { get; set; } = new List<AssertTreeEntityDTO>(); // Initialize children as an empty list
    }


    public class TreeNode
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("id")]
        public string id { get; set; }
        [JsonPropertyName("icon")]
        public string icon { get; set; }
        [JsonPropertyName("isUnit")]
        public bool isUnit { get; set; }
        [JsonPropertyName("haveRightToView")]

        public bool haveRightToView { get; set; }
        [JsonPropertyName("selected")]
        public bool selected { get; set; }
        [JsonPropertyName("indeterminate")]
        public bool indeterminate { get; set; }
        [JsonPropertyName("children")]
        public List<TreeNode> children { get; set; } = new List<TreeNode>();
    }
}
