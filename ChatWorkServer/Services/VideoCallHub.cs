using ChatWorkServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace ChatWorkServer.Services
{
    public class VideoCallHub: Hub
    {
        private readonly ChatDbContext _context;

        public VideoCallHub(ChatDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (!user.Identity.IsAuthenticated)
            {
                Console.WriteLine("Unauthenticated user attempted to connect.");
                await base.OnConnectedAsync();
                return;
            }

            var userIdStr = user.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                Console.WriteLine("Failed to retrieve user ID.");
                await base.OnConnectedAsync();
                return;
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                Console.WriteLine("Invalid user ID format.");
                await base.OnConnectedAsync();
                return;
            }

            // Đánh dấu các kết nối cũ là offline
            var oldConnections = await _context.Connections
                .Where(x => x.IsOnline && x.UserId == userId && x.Type == 2)
                .ToListAsync();

            oldConnections.ForEach(conn => conn.IsOnline = false);
            _context.UpdateRange(oldConnections);

            // Thêm kết nối mới
            var connectionId = Context.ConnectionId;
            var newConnection = new ConnectionModel
            {
                ConnectionId = connectionId,
                UserId = userId,
                StartedAt = DateTime.Now,
                IsOnline = true,
                Type = 2
            };

            await _context.Connections.AddAsync(newConnection);
            await _context.SaveChangesAsync();

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            var conn = await _context.Connections.FirstOrDefaultAsync(x => x.ConnectionId == connectionId);
            if (conn != null)
            {
                conn.IsOnline = false;
                conn.EndedAt = DateTime.Now;
                _context.Update(conn);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Connection not found for ID: {connectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendOffer(string offer, string targetConnectionId)
        {
            if (string.IsNullOrEmpty(targetConnectionId))
            {
                Console.WriteLine("Target connection ID is null or empty.");
                return;
            }
            Console.WriteLine("SendOffer to Group " + targetConnectionId);
            await Clients.Client(targetConnectionId).SendAsync("offer", offer);
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);// Bỏ group 
        }

        public async Task SendAnswer(string answer, string targetConnectionId)
        {
            if (string.IsNullOrEmpty(targetConnectionId))
            {
                Console.WriteLine("Target connection ID is null or empty.");
                return;
            }
            await Clients.Client(targetConnectionId).SendAsync("answer", answer);
        }

        public async Task SendICECandidate(string candidate, string targetConnectionId)
        {
            if (string.IsNullOrEmpty(targetConnectionId))
            {
                Console.WriteLine("Target connection ID is null or empty.");
                return;
            }
            await Clients.Client(targetConnectionId).SendAsync("ice-candidate", candidate);
        }
    }
}
