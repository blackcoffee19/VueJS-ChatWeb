namespace ChatWorkServer.DTOs
{
    public class UserDto
    {
        public int UsID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public string? Avatar { get; set; }
        public bool IsAdmin { get; set; }
        public virtual ICollection<MemberGroupDto> GroupsJoined { get; set; } = new List<MemberGroupDto>();
        public virtual ICollection<GroupChatDto> GroupCreated { get; set; } = new List<GroupChatDto>();
        public virtual ICollection<ChatDto> ChatsSend { get; set; } = new List<ChatDto>();
        public UserDto(int id, string username, string password, string fullname, string av, bool isAdmin)
        {
            UsID = id;
            Username = username;
            Password = password;
            Fullname = fullname;
            Avatar = av;
            IsAdmin = isAdmin;
        }
    }
}
