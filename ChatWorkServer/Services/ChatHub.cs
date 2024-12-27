﻿using ChatWorkServer.Common;
using ChatWorkServer.DTOs;
using ChatWorkServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System;
using System.Security.Claims;
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
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            int userId = 0;
            if (user.Identity.IsAuthenticated)
            {
                var userIdStr = user.FindFirst(ClaimTypes.Sid)?.Value;
                if (userIdStr != null)
                {
                    userId = Int32.Parse(userIdStr);
                    List<ConnectionModel> oldConnections = await _context.Connections.Where(x => x.IsOnline && x.UserId == userId).ToListAsync();
                    foreach (var item in oldConnections)
                    {
                        item.IsOnline = false;
                        if(_context.Update(item)!=null) _context.SaveChanges();
                    }
                    var connectionId = Context.ConnectionId;
                    ConnectionModel conn = new ConnectionModel();
                    conn.ConnectionId = connectionId;
                    conn.UserId = userId;
                    conn.StartedAt = DateTime.Now;
                    conn.IsOnline = true;
                    _context.Connections.Add(conn);
                    await _context.SaveChangesAsync();
                }
                else {
                    return;
                }
            }
            else
            {
                Console.WriteLine("Unauthenticated user attempted to connect.");
                return;
            }

            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User.FindFirst(ClaimTypes.Sid)?.Value; // Lấy UserId nếu có xác thực
            if (userId == null)
            {
                return base.OnDisconnectedAsync(exception);
            }
            // Cập nhật trạng thái offline khi người dùng thoát
            ConnectionModel conn = _context.Connections.FirstOrDefault(x => x.ConnectionId == connectionId);
            if (conn != null)
            {
                conn.IsOnline = false;
                conn.EndedAt = DateTime.Now;
                _context.Update(conn);
                _context.SaveChanges();
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task SendMessage(string text, int userId, int groupId, string groupName)
        {

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
        public async Task SendRequest(int? userId, int? requirementId) {
            if (userId == null || requirementId == null)
            {
                throw new ArgumentException("User or message cannot be null or empty");
            }
            bool checkUs = _context.Users.Any(x => x.UsID == userId);
            bool checkOnl = _context.Connections.Any(x => x.UserId == userId & x.IsOnline);
            if (checkUs&& checkOnl) {
                ConnectionModel connection = await _context.Connections.FirstAsync(x => x.UserId == userId && x.IsOnline);
                Console.WriteLine("Connection to: "+connection.ConnectionId);
                RequirementModel? req = await _context.Requirements.FirstOrDefaultAsync(x => x.RId == requirementId);
                if(req!=null)
                {
                    await Clients.Client(connection.ConnectionId).SendAsync("ReceiveRequest", req.ToJson());
                }
            }
        }
    }
}
