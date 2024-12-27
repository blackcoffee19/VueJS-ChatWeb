using ChatWorkServer.Common;
using ChatWorkServer.DTOs;
using ChatWorkServer.Models;
using ChatWorkServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatWorkServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly WebSocketHandler _webSocketHandler;
        public ChatController(ChatDbContext context)
        {
            _context = context;
            _webSocketHandler = new WebSocketHandler();
        }
        public class ChatForm { 
            public string TextMessage { get; set; }
            public int UserId { get; set; }
            public int GroupId { get; set; }
            
        }
        [HttpGet("{grId}")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetListChats( int grId)
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid);
            int  usId = 0;
            if (userId != null)
            {
                usId = int.Parse(userId);
            }
            else {
                return BadRequest();
            }
            bool checkMem = await _context.Memebers.AnyAsync(x => x.GroupId == grId && x.UserId == usId);
            if (!checkMem) {
                return BadRequest();
            }
            var result = await _context.Chats.Where(x=> x.GroupId == grId).ToListAsync();
            ChatModel? lastestChat = result.FirstOrDefault(x => x.UserId != usId && !x.IsSeen);
            if (lastestChat != null) {
                lastestChat.IsSeen = true;
                _context.Chats.Update(lastestChat);
                await _context.SaveChangesAsync();
            }
            List<ChatDto> ListChat = new List<ChatDto>();
            foreach (var item in result)
            {
                ChatDto chat = new ChatDto(item.Id, item.TextMessage, item.ImgMessage, item.CreatedDate, item.UserId, item.GroupId, item.IsSeen,TUtility.GetTimeSpan(item.CreatedDate));
                UsersModel usr = await _context.Users.FirstOrDefaultAsync(x => x.UsID == item.UserId);
                chat.userImg = usr.Avatar;
                ListChat.Add(chat);
            }
            return Ok(ListChat);
        }
        [HttpPost]
        public async Task<ActionResult> AddMessage(ChatForm chat) {
            Console.WriteLine(chat.TextMessage);
            try {
                ChatModel chatModel = new ChatModel(chat.TextMessage, chat.UserId, chat.GroupId);
                _context.Chats.Add(chatModel);
                await _context.SaveChangesAsync();
                return Ok();
                
            } catch (Exception ex) { 
                return BadRequest();
            }
        }
        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                await _webSocketHandler.HandleSocketAsync(HttpContext);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
