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

        // GET: api/Messages conversations list of logged user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var senderId = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(), token);
            //var messages = _context.Messages.Where(m => m.SenderId == senderId).Select(m=>m.ReceiverId).Distinct().OrderBy(m=>m.Date);
            var conversations =
                  await  (from m in (
                            from m in _context.Messages
                            where m.SenderId == senderId
                            group m by m.ReceiverId into grp
                            select grp.First())
                         // join m2 in _context.Messages on m.ReceiverId equals m2.ReceiverId
                     orderby m.Date descending
                     select m).ToListAsync();
                    //select new Message
                    //{
                    //    Date = m2.Date,
                    //    Id = m2.Id,
                    //    SenderId = m2.SenderId,
                    //    ReceiverId = m2.ReceiverId,
                    //    MessageContent = m2.MessageContent
                    //};


                            
            return conversations;
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


        // GET: api/Messages/5 //messages between logged user and userid
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessage(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var senderId = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(), token);
            Console.WriteLine(token);
            Console.WriteLine(senderId);
            if (senderId.HasValue)
            {
                var senderIdInt = senderId.Value;
                try
                {
                    var message = await _context.Messages.Where(s => (s.ReceiverId == senderIdInt && s.SenderId == id)
                                || (s.ReceiverId == id && s.SenderId == senderIdInt)).OrderBy(c => c.Date).ToListAsync();
                    return message;
                }
                catch
                {
                    return NotFound();
                }
            }
            return NotFound();

        }


    }
}
