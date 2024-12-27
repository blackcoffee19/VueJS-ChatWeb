
namespace ChatWorkServer.DTOs
{
    public class NotificationDto
    {
        public int rId { get; set; }
        public int userId { get; set; }
        public string? image { get; set; }
        public string? fullname { get; set; }
        public DateTime createdDate { get; set; }
        public int type { get; set; }

        public NotificationDto(int rId, int userId, string? image, string? fullname, DateTime createdDate, int type)
        {
            this.rId = rId;
            this.userId = userId;
            this.image = image;
            this.fullname = fullname;
            this.createdDate = createdDate;
            this.type = type;
        }
    }
}
