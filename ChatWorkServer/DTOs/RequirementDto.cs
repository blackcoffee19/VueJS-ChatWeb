namespace ChatWorkServer.DTOs
{
    public class RequirementDto
    {
        public int Type { get; set; }
        public bool Status { get; set; } = false;
        public int AssigneeId { get; set; }
    }
}
