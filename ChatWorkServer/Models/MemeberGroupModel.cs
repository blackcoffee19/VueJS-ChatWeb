using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWorkServer.Models
{
    [Table("MemeberGroupTb")]
    public class MemeberGroupModel
    {
        [Key]
        public int MemId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GroupId { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public UsersModel User { get; set; }
        public GroupChatModel GroupChat { get; set; }
        public MemeberGroupModel() { }
        public MemeberGroupModel(int Id,int UserId, int GroupId, DateTime date) {
            this.MemId = Id;
            this.UserId = UserId;
            this.GroupId = GroupId; 
            this.CreatedDate = date;
        }
    }
}
