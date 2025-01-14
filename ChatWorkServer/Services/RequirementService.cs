using ChatWorkServer.DTOs;
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
        public List<RelationshipDto> ListFriend(int UserId, int? User2Id)
        {
            List<int> RelationshipOfUser = (from r in _context.Relationships where (r.User_1_Id == UserId || r.User_2_Id == UserId) && r.UserDeleted==null && r.Status == true && r.Type ==1  select r.User_1_Id != UserId ?r.User_1_Id: r.User_2_Id ).ToList();
            List<RelationshipDto> res = new List<RelationshipDto>();
            int UserFound = UserId;
            if (User2Id != null) {
                UserFound = (int) User2Id ;
            }
            var ressult =(from r in _context.Relationships
                       join _u2 in _context.Users on r.User_2_Id equals _u2.UsID into u2
                       from _u2 in u2.DefaultIfEmpty()
                       join _u1 in _context.Users on r.User_1_Id equals _u1.UsID into u1
                       from _u1 in u1.DefaultIfEmpty()
                       where (_u1.UsID == UserFound || _u2.UsID == UserFound) && r.Status == true && r.Type == 1 && r.UserDeleted == null
                      select new 
                       {
                           RelaId = r.RelaId,
                           Type = r.Type,
                           User_1_Id = r.User_1_Id,
                           User_2_Id = r.User_2_Id,
                           User1 = (_u1 !=null) && (_u2!=null) && _u2.UsID == UserFound ? new UsersModel(_u1.UsID, _u1.Username??"", _u1.Fullname??"",  _u1.Avatar??"", DateTime.Now): new UsersModel(_u2.UsID, _u2.Username??"",_u2.Fullname??"", _u2.Avatar??"", DateTime.Now),
                           Counting = r.Counting,
                           Status = r.Status,
                           CreatedDate = r.CreatedDate,
                           UserCreated = r.UserCreated
                       }).ToList();
            foreach (var relation in ressult)
            {
                RelationshipDto relationDto = new RelationshipDto(relation.RelaId, relation.Type, relation.User_1_Id, relation.User_2_Id, relation.Counting, relation.Status, relation.UserCreated, relation.CreatedDate);
                relationDto.User = new UserDto(relation.User1.UsID, relation.User1.Username??"", "", relation.User1.Fullname, relation.User1.Avatar, relation.User1.IsAdmin);
                relationDto.User.IsFriend = relation.Type == 1 && RelationshipOfUser.Contains(relation.User1.UsID);
                
                res.Add(relationDto);
            }
            return res;
        }
        public GroupChatDto? getGroupChatWithUser(int User1Id, int? User2Id) {
            GroupChatDto groupChatDto = new GroupChatDto();
            if (User2Id != null && User1Id != User2Id)
            {
                var group = (from g in _context.GroupChats
                             join _m1 in _context.Memebers on g.GrId equals _m1.GroupId into m1
                             from _m1 in m1.DefaultIfEmpty()
                             join _m2 in _context.Memebers on g.GrId equals _m2.GroupId into m2
                             from _m2 in m2.DefaultIfEmpty()
                             where _m1.UserId == User1Id && _m2.UserId == User2Id && g.UserDeleted == null
                             select new
                             {
                                 GrId = g.GrId,
                                 Name = g.Name,
                                 Code = g.Code,
                                 UserBlocked = g.UserBlocked,
                                 BlockedDate = g.BlockedDate,
                                 UserDeleted = g.UserDeleted,
                                 DeletedDate = g.DeletedDate
                             }
                             ).FirstOrDefault();
                if (group != null)
                {
                    groupChatDto = new GroupChatDto(group.GrId, group.Name, group.Code, group.UserBlocked, group.BlockedDate, group.UserDeleted, group.DeletedDate);
                }
                
            }
            return groupChatDto;
        }
    }
}
