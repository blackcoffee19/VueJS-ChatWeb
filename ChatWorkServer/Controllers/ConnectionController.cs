using ChatWorkServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConnectionController : ControllerBase
    {
        private readonly ChatDbContext _context;
        public ConnectionController(ChatDbContext context)
        {
            _context = context;
        }
        [HttpPost("GetUserConnection")]
        public async Task<ActionResult<IEnumerable<ConnectionModel?>>> GetUserConnection(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            int usId = 0;
            if (userId == null || !int.TryParse(userId, out usId) || id == 0)
            {
                return BadRequest();
            }

            MemeberGroupModel? mem = await _context.Memebers.FirstOrDefaultAsync(x => x.GroupId == id && x.UserId != int.Parse(userId));

            ConnectionModel? connection = await _context.Connections.FirstOrDefaultAsync(x => x.UserId == mem.UserId && x.IsOnline && x.Type==2);

            return Ok(connection);
        }
    }
}
