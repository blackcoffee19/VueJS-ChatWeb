using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWorkServer.Models
{
    [Table("UsersTb")]
    public class UsersModel
    {
        [Key]
        public int UsID { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Fullname { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public int? UserCreated { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UserDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? UserBlocked { get; set; }
        public DateTime? BlockedDate { get; set; }

        public virtual ICollection<MemeberGroupModel> GroupsJoined { get; set; }=new List<MemeberGroupModel>();
        public virtual ICollection<GroupChatModel> GroupCreated { get; set; } = new List<GroupChatModel>();
        public virtual ICollection<ChatModel> ChatsSend { get; set; } =new List<ChatModel>();
        public virtual ICollection<RelationshipModel> UserRelationships { get; set; } =new List<RelationshipModel>();
        public UsersModel() {  }
        public UsersModel(int Id,string Username, string Fullname, string Password, bool IsAdmin, int userCreated,DateTime date, string avatar) {
            this.UsID = Id;
            this.Username = Username;
            this.Fullname = Fullname;
            this.Password = Password;
            this.IsAdmin = IsAdmin;
            this.UserCreated = userCreated;
            this.CreatedDate = date;
            this.Avatar = avatar;
        }
    }
}
