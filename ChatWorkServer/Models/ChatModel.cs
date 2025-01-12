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
        public DateTime? DeletedDate { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public bool IsSeen { get; set; }
        public UsersModel UserChat { get; set; }
        public GroupChatModel GroupChat { get; set; }
        public ChatModel() { }
        public ChatModel(int id, string text, int user, int group, bool isSeen) {
            this.Id = id;
            this.TextMessage = text;
            this.UserId = user;
            this.GroupId = group;
            this.IsSeen = isSeen;
            this.CreatedDate = DateTime.Now;
        }

        public ChatModel(string? textMessage, int userId, int groupId)
        {
            TextMessage = textMessage;
            UserId = userId;
            GroupId = groupId;
            CreatedDate = DateTime.Now;
            IsSeen = false;
        }
    }
}
