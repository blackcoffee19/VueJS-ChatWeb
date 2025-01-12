using System.ComponentModel.DataAnnotations;

namespace ChatWorkServer.Models
{
    public class RelationshipModel
    {
        [Key]
        public int RelaId { get; set; }
        public int Type { get; set; }
        public int User_1_Id { get; set; }
        public int User_2_Id { get; set; }
        public int Counting { get; set; }
        public bool Status { get; set; }
        public int UserCreated { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UserModified { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public UsersModel User1 { get; set; }
        public UsersModel User2 { get; set; }
        public RelationshipModel() { }
        public RelationshipModel(int id, int type, int user1, int user2, bool status, int count,int userCreated, DateTime createdDate)
        {
            this.RelaId = id;
            this.Type = type;
            this.User_1_Id = user1;
            this.User_2_Id = user2;
            this.Counting = count;
            this.Status = status;
            this.UserCreated = userCreated;
            this.CreatedDate = createdDate;
        }
    }
}
