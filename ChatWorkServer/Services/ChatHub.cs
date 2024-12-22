using ChatWorkServer.Common;
using ChatWorkServer.DTOs;
using ChatWorkServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System;
using System.Threading.Tasks;

namespace ChatWorkServer.Services
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;
        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendMessage(string text, int userId, int groupId, string groupName)
        {

            Console.WriteLine(text);
            try
            {
                List<ChatModel> listPrevChats = await _context.Chats.Where(x => x.UserId != userId && x.GroupId == groupId && !x.IsSeen).ToListAsync();
                foreach (var item in listPrevChats)
                {
                    item.IsSeen = true;
                    _context.Update(item);
                }
                ChatModel chatModel = new ChatModel(text, userId, groupId);
                _context.Chats.Add(chatModel);
                await _context.SaveChangesAsync();
                ChatDto chat = new ChatDto(chatModel.Id, chatModel.TextMessage, chatModel.ImgMessage, chatModel.CreatedDate, chatModel.UserId, chatModel.GroupId, chatModel.IsSeen, TUtility.GetTimeSpan(chatModel.CreatedDate));
                UsersModel usr = await _context.Users.FirstOrDefaultAsync(x => x.UsID == chatModel.UserId);
                chat.userImg = usr.Avatar;
                // Gửi tin nhắn đến tất cả các client đã kết nối
                await Clients.Group(groupName).SendAsync("ReceiveMessage", chat.ToJson());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
