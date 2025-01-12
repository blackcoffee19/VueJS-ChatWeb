using ChatWorkServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWorkServer.Services
{
    public class RequirementService
    {
        private readonly ChatDbContext _context;
        public RequirementService(ChatDbContext context)
        {
            _context = context;
        }

        public void AddFriendAction(int ReqId)
        {
            _context.Database.ExecuteSqlRaw("EXEC sp_AddFriends @ReqId={0}", ReqId);
        }
        public void UnFriendAction(int ReqId, int UserId)
        {
            try
            {
                _context.Database.ExecuteSqlRaw("EXEC sp_Unfriend @Userid={0}, @UserReq={1}", UserId, ReqId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
