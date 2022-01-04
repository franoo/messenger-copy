using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Authentication;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MyDBContext _context;
        private ITokenService _tokenService;
        private readonly IConfiguration _config;

        public MessagesController(MyDBContext context, ITokenService tokenService, IConfiguration config)
        {
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(MessageRequest message)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var senderId = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(),token);
            Console.WriteLine(token);
            Console.WriteLine(senderId);
            if (senderId.HasValue)
            {
                int id = senderId.Value;
                Message messageToDB = new Message
                {
                    Date = DateTime.Now,
                    MessageContent = message.MessageContent,
                    ReceiverId = message.ReceiverId,
                    SenderId = id
                };
                _context.Messages.Add(messageToDB);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMessage", new { id = messageToDB.Id }, messageToDB);
            }
            return null;
        }


        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }


    }
}
