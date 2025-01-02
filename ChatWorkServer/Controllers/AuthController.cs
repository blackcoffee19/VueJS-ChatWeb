using ChatWorkServer.Common;
using ChatWorkServer.Models;
using ChatWorkServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ChatDbContext _context;
        public AuthController(ChatDbContext context) { 
            _context = context;
        }
        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class RegisterModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Fullname { get; set; }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var UserCheck = _context.Users.FirstOrDefaultAsync(x => x.Username == login.Username && TUtility.GetMD5(login.Password) == x.Password).Result;
            // Giả lập xác thực người dùng
            if (UserCheck != null)
            {
                TokenService tokenService = new TokenService();
                string token = tokenService.GenerateToken(UserCheck);
                return Ok(new { Token = token, UserId = UserCheck.UsID});
            }

            return Unauthorized();
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel register) {
            if (register.Username.Trim() == "" 
                || register.Password.Trim() == ""
                || register.Fullname.Trim() == "") {
                return BadRequest();
            }
            UsersModel? userCheck = _context.Users.FirstOrDefault(x => x.Username == register.Username);
            if (userCheck != null) {
                return BadRequest();
            }
            UsersModel user = new UsersModel();
            user.Username = register.Username;
            user.Password = TUtility.GetMD5(register.Password);
            user.Fullname = register.Fullname;
            _context.Add(user);
            _context.SaveChanges();
            if (user.UsID > 0) { 
                TokenService tokenService = new TokenService();
                string token = tokenService.GenerateToken(user);
                return Ok(new { Token = token, UserId = user.UsID });
            }
            return BadRequest();
        }
    }
}
