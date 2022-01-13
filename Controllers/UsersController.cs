using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDBContext _context;
        private ITokenService _tokenService;
        private readonly IConfiguration _config;

        public UsersController(MyDBContext context, ITokenService tokenService, IConfiguration config)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(UserLogin model)
        {
            try
            {
                var response = _context.Users.SingleOrDefault(x => x.username == model.UserName && x.PasswordHash == model.Password);//_userService.Authenticate(model);
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


        // GET: api/Users1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
           // var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
           // var userId = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(), token);
            if (id != user.id)
            {
                return BadRequest();
            }
            var userDb = await _context.Users.FindAsync(id);
            userDb.signalRconnectionId = user.signalRconnectionId;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.id }, user);
        }


        [HttpPost("updateconnectionid")]
        public async Task<ActionResult<User>> UpdateConnectionId(ConnectionId connection)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var id = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(), token);
            if (id.HasValue)
            {
                var userDb = await _context.Users.FindAsync(id);
                userDb.signalRconnectionId = connection.connectionId;

                _context.Entry(userDb).State = EntityState.Modified;
               // _context.Users.Add(userDb);
                //await _context.SaveChangesAsync();

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id.Value))
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
            return BadRequest();
        }

        // DELETE: api/Users1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }
    }
}
