namespace ChatWorkServer.Models
{
    public class UserDto
    {
        public int UsID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public string? Avatar { get; set; }
        public bool IsAdmin { get; set; }

        public UserDto(int id, string username, string password, string fullname,string av, bool isAdmin) {
            this.UsID = id;
            this.Username = username;
            this.Password = password;
            this.Fullname = fullname;
            this.Avatar = av;
            this.IsAdmin = isAdmin;
        }
    }
}
