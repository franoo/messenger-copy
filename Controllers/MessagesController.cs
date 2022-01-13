using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Authentication;
using WebApi.Helpers;
using WebApi.Hubs;
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
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesController(IHubContext<ChatHub> hubContext,MyDBContext context, ITokenService tokenService, IConfiguration config)
        {
            _hubContext = hubContext;
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        // GET: api/Messages conversations list of logged user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<int>>> GetMessages()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var SenderIdInt = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(), token);
            var query =
                _context.Messages.OrderBy(x => x.Date).Take(10);

            // from row in _context.Messages
            // group row by row.ConversationId
            //into g
            // select g.OrderBy(n => n.Date).ToList();
            // _context.Messages.GroupBy(x => x.ConversationId).Select(x => x.OrderByDescending(y => y.Date).FirstOrDefault());
            //{
            //var conversations = await query.ToListAsync();
            //})
            //_context.Set<Message>()
            //.Where(t => _context.Messages.Contains(t.ConversationId))
            //.Select(t => t.ConversationId).Distinct() // <--
            //.SelectMany(key => context.Set<DbDocument>().Where(t => t.SenderId == key) // <--
            //    .OrderByDescending(t => t.InsertedDateTime).Take(10)
            //);
            //from m in _context.Messages
            //let msgTo = m.ReceiverId == id
            //let msgFrom = m.SenderId == id
            //where msgTo || msgFrom
            //group m by msgTo ? m.SenderId : m.ReceiverId into g
            //select g.OrderByDescending(x => x.Date).First();
            var conversations = await query.ToListAsync();
            return  Ok(conversations);
            
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(MessageRequest message)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var senderId = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(),token);
            //Console.WriteLine(token);
            //Console.WriteLine(senderId);
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

                var  userWithSignalConnectionId = await _context.Users.FindAsync(message.ReceiverId);
                string conn = userWithSignalConnectionId.signalRconnectionId;
                if (conn != null)
                {
                    await _hubContext.Clients.Client(conn).SendAsync("ReceiveOne", messageToDB.Id, messageToDB.SenderId, messageToDB.ReceiverId, messageToDB.MessageContent, messageToDB.Date);
                }
                return CreatedAtAction("GetMessage", new { id = messageToDB.Id }, messageToDB);
            }
            return BadRequest();
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
