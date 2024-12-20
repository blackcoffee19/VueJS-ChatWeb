﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatWorkServer.Models;
using Microsoft.AspNetCore.Authorization;
using ChatWorkServer.Common;
using System.Security.Claims;

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

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            List<UserDto> users = new List<UserDto>();
            await _context.Users.ForEachAsync(x => users.Add(new UserDto(x.UsID, x.Username, "", x.Fullname, x.Avatar, x.IsAdmin)));
            return Ok(users);
        }
        [HttpGet("profie")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetProfie()
        {
            var userId = User.FindFirstValue(ClaimTypes.Sid); // Lấy Id từ Claims
            var username = User.Identity?.Name; // Lấy Username
            if (userId != null) {
                int id = 0; 
                bool parse = int.TryParse(userId, out id);
                UsersModel user = await _context.Users.FirstAsync(x=> x.UsID == id);
                UserDto userDto = new UserDto(user.UsID,user.Username,"", user.Fullname, user.Avatar, user.IsAdmin);
                return Ok(userDto);
            }
            return BadRequest();
        }
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUsersModel(int id)
        {
            var usersModel = await _context.Users.FindAsync(id);

            if (usersModel == null)
            {
                return NotFound();
            }
            UserDto user = new UserDto(usersModel.UsID, usersModel.Username, "", usersModel.Fullname, usersModel.Avatar, usersModel.IsAdmin);
            return user;
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

        private bool UsersModelExists(int id)
        {
            return _context.Users.Any(e => e.UsID == id);
        }
    }
}
