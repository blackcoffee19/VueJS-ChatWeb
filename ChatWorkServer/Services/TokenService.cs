using ChatWorkServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatWorkServer.Services
{
    public class TokenService
    {
        public string GenerateToken(UsersModel user)
        {
            
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username!),
            new Claim(ClaimTypes.Sid, user.UsID.ToString()), // Thêm UserId vào Claims
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin": "User"),
            new Claim(ClaimTypes.GivenName, user.Fullname!)// Thêm thông tin khác nếu cần
            };
            // Khóa bí mật
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Hinata Sohara is the prettiest girl"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: "hinatasohara",
                audience: "xg",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
