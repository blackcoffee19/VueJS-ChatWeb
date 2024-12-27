using System.ComponentModel.DataAnnotations;

namespace ChatWorkServer.Models
{
    public class ConnectionModel
    {
        [Key]
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public bool IsOnline { get; set; }  
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public ConnectionModel() { }
    }
}
