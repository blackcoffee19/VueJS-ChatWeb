using ChatWorkServer.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ChatWorkServer.Models
{
    public class RequirementModel
    {
        [Key]
        public int RId { get; set; }
        public int Type { get; set; }
        public bool Status { get; set; } = false;
        public int AssigneeId { get; set; }
        public int RequesterId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public RequirementModel() { }
    }
}
