using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Authentication;
using WebApi.Helpers;
using WebApi.Hubs;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly MyDBContext _context;
        private ITokenService _tokenService;
        private readonly IConfiguration _config;

        public ChatController(IHubContext<ChatHub> hubContext, MyDBContext context, ITokenService tokenService, IConfiguration config)
        {
            _hubContext = hubContext;
            _context = context;
            _tokenService = tokenService;
            _config = config;
        }

        [Route("send")]                                           //path looks like this: https://localhost:44379/api/chat/send
        [HttpPost]
        public async Task<IActionResult> SendRequestAsync([FromBody] MessageRequest msg)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var senderId = _tokenService.ValidateToken(_config["Jwt:Key"].ToString(), token);
            Console.WriteLine(token);
            Console.WriteLine(senderId);
            if (senderId.HasValue)
            {
                int id = senderId.Value;
                Message messageToDB = new Message
                {
                    Date = DateTime.Now,
                    MessageContent = msg.MessageContent,
                    ReceiverId = msg.ReceiverId,
                    SenderId = id
                };
                _context.Messages.Add(messageToDB);
                await _context.SaveChangesAsync();

                //await _hubContext.Clients.All.SendAsync("ReceiveOne", messageToDB);
            }
            return Ok();
        }
    }
}