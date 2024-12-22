using System.Diagnostics.CodeAnalysis;

namespace ChatWorkServer.DTOs
{
    public class ChatDto
    {
        public int id { get; set; }
        [AllowNull]
        public string? textMessage { get; set; }
        [AllowNull]
        public string? imgMessage { get; set; }
        public DateTime createdDate { get; set; }
        public int userId { get; set; }
        public int groupId { get; set; }
        public bool isSeen { get; set; }
        [AllowNull]
        public UserDto userChat { get; set; }
        [AllowNull]
        public GroupChatDto groupChat { get; set; }
        [AllowNull]
        public string timeSpan { get; set; }
        [AllowNull]
        public string userImg { get; set; }
        public ChatDto() { }
        public ChatDto(int id, string text, string img, DateTime date, int user, int group, bool isSeen, string time) {
            this.id = id;
            this.textMessage = text;
            this.imgMessage = img;
            this.createdDate = date;
            this.userId = user;
            this.groupId = group;
            this.isSeen = isSeen;
            this.timeSpan = time;
        }
        public ChatDto(string text, int userId, int groupId) {
            this.textMessage = text;
            this.userId = userId;
            this.groupId = groupId;
        }
    }
}
