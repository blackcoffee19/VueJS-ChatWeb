namespace ChatWorkServer.DTOs
{
    public class GroupChatDto
    {
        public GroupChatDto()
        {
        }

        public GroupChatDto(int grId, string? name, int userCreated, DateTime createdDate, string code)
        {
            GrId = grId;
            Name = name;
            UserCreated = userCreated;
            CreatedDate = createdDate;
            Code = code;
        }

        public GroupChatDto(int grId, string name, string code, int? userBlocked, DateTime? blockedDate, int? userDeleted, DateTime? deletedDate)
        {
            GrId = grId;
            Name = name;
            Code = code;
            UserBlocked = userBlocked;
            BlockedDate1 = blockedDate;
            UserDeleted = userDeleted;
            DeletedDate = deletedDate;
        }

        public int GrId { get; set; }
        public string Image { get; set; }
        public string? Name { get; set; }
        public string? SubName { get; set; }
        public int UserCreated { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime BlockedDate { get; set; }
        public virtual ICollection<MemberGroupDto> MemeberGroup { get; set; } = new List<MemberGroupDto>();
        public ChatDto ChatsSend { get; set; }
        public int? UserBlocked { get; }
        public DateTime? BlockedDate1 { get; }
        public int? UserDeleted { get; }
        public DateTime? DeletedDate { get; }
    }
}
