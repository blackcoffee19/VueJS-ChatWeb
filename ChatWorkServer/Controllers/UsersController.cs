using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatWorkServer.Models;
using Microsoft.AspNetCore.Authorization;
using ChatWorkServer.Common;
using System.Security.Claims;
using ChatWorkServer.DTOs;
using System.Data.Common;
using ChatWorkServer.Services;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
namespace ChatWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public UsersController(ChatDbContext context)
        {
            _context = context;
        }
        public class ResponseListGroupChatAndNotification
        {
            public List<GroupChatDto> groupChats { get; set; }
            public int notifications { get; set; }
        }
        [HttpPost("profie")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetProfie(string? ParamUser)
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid); // Lấy Id từ Claims
            if (ParamUser != null) {
                int id = 0;
                bool parse = int.TryParse(userId, out id);
                UsersModel? user = await _context.Users.FirstOrDefaultAsync(x => x.Username == ParamUser);
                if (user == null)
                {
                    return BadRequest();
                }
                UserDto userDto = new UserDto(user.UsID, user.Username, "", user.Fullname, user.Avatar, user.IsAdmin);
                userDto.IsFriend = _context.Relationships.Any(x => ((x.User_1_Id == user.UsID && x.User_2_Id == id) || (x.User_2_Id == user.UsID && x.User_1_Id == id)) && x.UserDeleted == null && x.Status);
                userDto.IsSentRequest = _context.Requirements.Any(x => x.RequesterId == id && x.AssigneeId == user.UsID && x.Type == 1 && x.Status);
                userDto.IsReceivedRequest = _context.Requirements.Any(x => x.AssigneeId == id && x.RequesterId == user.UsID && x.Type == 1 && x.Status);
                List<RelationshipDto> listData = new RequirementService(_context).ListFriend(id, user.UsID);
                userDto.Relationships = listData;
                userDto.Group = new RequirementService(_context).getGroupChatWithUser(id, user.UsID);
                return Ok(userDto);
            }
            else if (ParamUser  ==null && userId != null) {
                int id = 0; 
                bool parse = int.TryParse(userId, out id);
                UsersModel? user = await _context.Users.FirstOrDefaultAsync(x=> x.UsID == id);
                if (user == null) {
                    return BadRequest();
                }
                UserDto userDto = new UserDto(user.UsID,user.Username,"", user.Fullname, user.Avatar, user.IsAdmin);
                List<RelationshipDto> listRelationDto = new RequirementService(_context).ListFriend(id, null);
                userDto.Relationships = listRelationDto;
                
                return Ok(userDto);
            }
            return BadRequest();
        }
        
        [HttpGet("friends")]
        public Task<ActionResult<IEnumerable<RelationshipDto>>> GetListFriend(int? usrId) {
            var UId = User.FindFirstValue(ClaimTypes.Sid);
            int id = 0;
            bool parse = int.TryParse(UId, out id);
            if (!parse)
            {
                return Task.FromResult<ActionResult<IEnumerable<RelationshipDto>>>(BadRequest());
            }
            List<RelationshipDto> listData = new RequirementService(_context).ListFriend(id, usrId);
            return Task.FromResult<ActionResult<IEnumerable<RelationshipDto>>>(Ok(listData));
        }
        [HttpGet("ListGroupChat")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetListGroupChat()
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            if (userId != null)
            {
                int id = 0;
                bool parse = int.TryParse(userId, out id);
                List<int> ListGr = _context.Memebers.Where(x=> (x.UserId ==  id) && x.UserDeleted == null).Select(x=> x.GroupId).ToList();
                List<GroupChatModel> ListData= await _context.GroupChats.Where(x => ListGr.Contains(x.GrId)).ToListAsync();
                List<GroupChatDto> ListGroup = new List<GroupChatDto>();
                if (ListData.Count > 0) { 
                    ListData.ForEach( x =>
                    {
                        GroupChatDto group = new GroupChatDto(x.GrId, x.Name, x.UserCreated, x.CreatedDate, x.Code);
                        List<MemeberGroupModel> ListMemberGroups = _context.Memebers.Where(y => y.GroupId == x.GrId).ToList();
                        List<MemberGroupDto> ListMemberGroupsDto = new List<MemberGroupDto>();
                        ListMemberGroups.ForEach(y =>
                        {
                            ListMemberGroupsDto.Add(new MemberGroupDto(y.UserId, y.GroupId, x.UserCreated == y.UserId, y.CreatedDate));
                        });
                        group.MemeberGroup = ListMemberGroupsDto;
                        MemberGroupDto? mem = ListMemberGroupsDto.FirstOrDefault(y => y.UserId != id);
                        UsersModel user= _context.Users.FirstOrDefault(y => y.UsID == mem.UserId);
                        group.Image = user.Avatar;
                        group.SubName = user.Fullname;
                        try { 
                            ChatModel?  chatFo =  _context.Chats.Where(y => y.GroupId == x.GrId).OrderByDescending(y => y.CreatedDate).FirstOrDefault();
                            if (chatFo != null) { 
                                group.ChatsSend = new ChatDto(chatFo.Id, chatFo.TextMessage, chatFo.ImgMessage, chatFo.CreatedDate, chatFo.UserId, chatFo.GroupId, chatFo.IsSeen, TUtility.GetTimeSpan(chatFo.CreatedDate));
                            }                  
                        } catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }
                        ListGroup.Add(group);
                    });
                }
                List<RequirementModel> listReq = await _context.Requirements.Where(x => x.AssigneeId == id && x.Status && x.Type == 1).ToListAsync();
                ResponseListGroupChatAndNotification res = new ResponseListGroupChatAndNotification();
                res.groupChats = ListGroup;
                res.notifications = listReq.Count;
                return Ok( res);
            }
            return BadRequest();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsersModel(int id, UsersModel usersModel)
        {
            if (id != usersModel.UsID)
            {
                return BadRequest();
            }

            _context.Entry(usersModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUsersModel(UserDto userDto)
        {
            var userCheck = await _context.Users.FirstOrDefaultAsync(x => x.Username == userDto.Username);
            if (userCheck != null || userDto.Password == null|| userDto.Password.Trim().Length == 0) {
                return BadRequest();
            }
            UsersModel usersModel = new UsersModel();
            usersModel.Username = userDto.Username;
            usersModel.Password = TUtility.GetMD5(userDto.Password);
            usersModel.Fullname = userDto.Fullname;
            _context.Users.Add(usersModel);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsersModel", new { id = usersModel.UsID }, usersModel);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsersModel(int id)
        {
            var usersModel = await _context.Users.FindAsync(id);
            if (usersModel == null)
            {
                return NotFound();
            }

            _context.Users.Remove(usersModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("SearchUser")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUser(string search) {
            List<UserDto> lis = new List<UserDto>();
            var userId = User.FindFirstValue(ClaimTypes.Sid); // Lấy Id từ Claims
            if (userId == null) {
                return BadRequest();
            }
            int usId = int.Parse(userId);
            
            List<UsersModel> listUser= await _context.Users.Where(x => (x.Username.Contains(search) || x.Fullname.Contains(search)) && x.UsID != usId).ToListAsync();
            List<int> grId = await _context.Memebers.Where(y => y.UserId == usId).Select(y => y.GroupId).ToListAsync();
            listUser.ForEach(async x => {
                UserDto us = new UserDto(x.UsID, x.Username, "", x.Fullname, x.Avatar, x.IsAdmin);
                bool isSameGroupChat = _context.Relationships.Any(y => ((y.User_1_Id == x.UsID && y.User_2_Id == usId) || (y.User_2_Id == x.UsID && y.User_1_Id == usId)) && (y.UserDeleted ==null));
                us.IsFriend = isSameGroupChat;
                bool checkSendRequest = _context.Requirements.Any(z => z.RequesterId == usId && z.AssigneeId == x.UsID && z.Status && z.Type == 1);
                bool checkReceivedRequest = _context.Requirements.Any(z => z.AssigneeId == usId && z.RequesterId == x.UsID && z.Status && z.Type == 1);
                us.IsSentRequest = checkSendRequest;
                us.IsReceivedRequest = checkReceivedRequest;
                lis.Add(us);
            });
            return Ok(lis);
        }
        [HttpPost("AddRequest")]
        public ActionResult<int?> AddRequest(RequirementDto req) 
        {
            if (!UsersModelExists(req.AssigneeId)) {
                return BadRequest();
            }
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            int usId = 0;
            if(userId == null || !int.TryParse(userId, out usId))
            {
                return BadRequest();
            }
            bool check = _context.Requirements.Any(x => x.RequesterId == usId && x.AssigneeId == req.AssigneeId && x.Status);
            if(check)
            {
                return BadRequest();
            }
            RequirementModel newRequirement = new RequirementModel();
            newRequirement.AssigneeId = req.AssigneeId;
            newRequirement.Type = req.Type;
            newRequirement.RequesterId = usId;
            newRequirement.CreatedDate = DateTime.Now;
            newRequirement.Status = req.Status;
            _context.Requirements.Add(newRequirement);
            _context.SaveChanges();
            return Ok(newRequirement.RId);
        }
        [HttpPost("CancelRequest")]
        public IActionResult CancelRequest(RequirementDto req) {
            if (!UsersModelExists(req.AssigneeId))
            {
                return BadRequest();
            }
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            int usId = 0;
            if (userId == null || !int.TryParse(userId, out usId))
            {
                return BadRequest();
            }
            bool check = _context.Requirements.Any(x => x.RequesterId == usId && x.AssigneeId == req.AssigneeId && x.Status);
            if (check)
            {
                IEnumerable<RequirementModel> newRequirement = _context.Requirements.Where(x => x.RequesterId == usId && x.AssigneeId == req.AssigneeId && x.Status);
                foreach (var item in newRequirement)
                {
                    item.Status = false;
                    item.ModifiedDate = DateTime.Now;
                    _context.Update(item);
                }
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("ListNotifications", Name = "GetListNotifications")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetListNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            int usId = 0;
            if (userId == null || !int.TryParse(userId, out usId))
            {
                return BadRequest();
            }
            List<RequirementModel> listReq = await _context.Requirements.Where(x => x.AssigneeId == usId && x.Status && x.Type == 1).ToListAsync();
            List<NotificationDto> listNoti = new List<NotificationDto>();
            listReq.ForEach(x =>
            {
                UsersModel user = _context.Users.FirstOrDefault(y => y.UsID == x.RequesterId);
                NotificationDto noti = new NotificationDto(x.RId,user.UsID, user.Avatar, user.Fullname, x.CreatedDate, x.Type);
                listNoti.Add(noti);
            });
            return Ok(listNoti);
        }
        [HttpPost("AddFriendActions")]
        public async Task<ActionResult<int?>> AddFriendActions(int reqId, int isUserId, int actions)
        { //1 is accept add friend, 0 is un accept
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            int countRq = isUserId == 1? _context.Requirements.Count(x => x.AssigneeId == Int32.Parse(userId) && x.RequesterId == reqId && x.Status) : 0;
            if (countRq > 1)
            {
                return BadRequest();
            }
            RequirementModel? requirement = isUserId == 1? await _context.Requirements.FirstOrDefaultAsync(x => x.AssigneeId == Int32.Parse( userId )&& x.RequesterId == reqId && x.Status ) : await _context.Requirements.FindAsync(reqId);

            if (actions == 1 && requirement!=null) {
                new RequirementService(_context).AddFriendAction(requirement.RId);
                return Ok(1);
            }
            else if(actions == 0 && requirement != null)
            {
                requirement.Status = false;
                requirement.Type = 3;
                requirement.ModifiedDate = DateTime.Now;
                var update = _context.Update(requirement);
                if (update != null)
                {
                    _context.SaveChanges();
                    return Ok(2);
                }
            }
            return BadRequest();
        }
        [HttpPost("UnfriendAction")]
        public async Task<ActionResult<int?>> UnfriendAction(int userId) {
            var requId = User.FindFirstValue(ClaimTypes.Sid);
            if (requId != null ) {
                int usId = int.Parse(requId);
                new RequirementService(_context).UnFriendAction(usId, userId);
                return Ok();
            }
            return BadRequest();
        }
        private bool UsersModelExists(int id)
        {
            return _context.Users.Any(e => e.UsID == id);
        }
    }
}
