﻿namespace ChatWorkServer.DTOs
{
    public class GroupChatDto
    {
        public GroupChatDto(int grId, string? name, int userCreated, DateTime createdDate, string code)
        {
            GrId = grId;
            Name = name;
            UserCreated = userCreated;
            CreatedDate = createdDate;
            Code = code;
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
    }
}
