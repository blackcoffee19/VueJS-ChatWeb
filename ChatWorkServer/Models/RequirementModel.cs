using ChatWorkServer.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ChatWorkServer.Models
{
    public class RequirementModel
    {
        [Key]
        public int RId { get; set; }
        public int Type { get; set; } //1: Send Requirement, 2: Answer Requirement (accept), 3: Answer Requirement (no accept) , 4: Block 
        public bool Status { get; set; } = false;
        public int AssigneeId { get; set; }
        public int RequesterId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public RequirementModel() { }
    }
}
