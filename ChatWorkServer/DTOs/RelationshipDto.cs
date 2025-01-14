namespace ChatWorkServer.DTOs
{
    public class RelationshipDto
    {

        public int RelaId { get; set; }
        //Type  1 Friend
        //      2 Block 
        
        public int Type { get; set; }
        public int Counting { get; set; }
        public int User_1_Id { get; set; }
        public int User_2_Id { get; set; }
        public bool Status { get; set; }
        public int UserCreated { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UserModified { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public UserDto User { get; internal set; }
        public RelationshipDto() { }

        public RelationshipDto(int relaId, int type, int user_1_Id, int user_2_Id, int count,bool status, int userCreated, DateTime createdDate)
        {
            RelaId = relaId;
            Type = type;
            User_1_Id = user_1_Id;
            User_2_Id = user_2_Id;
            Counting = count;
            Status = status;
            UserCreated = userCreated;
            CreatedDate = createdDate;

        }
    }
}
