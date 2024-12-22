namespace ChatWorkServer.DTOs
{
    public class MemberGroupDto
    {
        public MemberGroupDto(int userId, int groupId, bool isAdmin, DateTime date)
        {
            UserId = userId;
            GroupId = groupId;
            IsAdmin = isAdmin;
            CreatedDate = date;
        }

        public int MemId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public DateTime CreatedDate { get; set; }

        public UserDto User { get; set; }
        public GroupChatDto GroupChat { get; set; }
        public bool IsAdmin { get; }
    }
}
