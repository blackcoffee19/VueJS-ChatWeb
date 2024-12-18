using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWorkServer.Models
{
    [Table("ChatTb")]
    public class ChatModel
    {
        [Key]
        public int Id { get; set; }
        public string? TextMessage { get; set; }
        public string? ImgMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public UsersModel UserChat { get; set; }
        public GroupChatModel GroupChat { get; set; }
        public ChatModel() { }
    }
}
