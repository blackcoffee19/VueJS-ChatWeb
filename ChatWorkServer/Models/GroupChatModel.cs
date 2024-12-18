using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWorkServer.Models
{
    [Table("GroupChatTb")]
    public class GroupChatModel
    {
        [Key]
        public int GrId { get; set; }
        public string? Name { get; set; }
        public int UserCreatedId { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime BlockedDate { get; set; }
        public virtual ICollection<MemeberGroupModel> MemeberGroup { get; set; } = new List<MemeberGroupModel>();
        public virtual ICollection<ChatModel> ChatsSend { get; set; } = new List<ChatModel>();
        public UsersModel UserCreated { get; set; }
        public GroupChatModel() { }
        public GroupChatModel(int Id,string Name, string Code, int UserCreated, DateTime date) { 
            this.GrId = Id;
            this.Name = Name;
            this.Code = Code;
            this.UserCreatedId = UserCreated;
            this.CreatedDate = date;
        }
    }
}
