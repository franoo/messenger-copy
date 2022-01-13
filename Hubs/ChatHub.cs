using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;                                         // using this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Hubs
{
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize]
    public class ChatHub : Hub                                              // inherit this
    {
        private readonly MyDBContext _context;
        //public Task SendMessage1(string user, string message)               // Two parameters accepted
        //{
        //    return Clients.All.SendAsync("ReceiveOne", user, message);    // Note this 'ReceiveOne' 
        //}
        public ChatHub(MyDBContext dBContext)
        {
            this._context = dBContext;
        }
        public string getConnectionId()
        {
            return Context.ConnectionId;
        }
        //public Task SendMessage(string receiverId, string message)               // Two parameters accepted
        //{
        //    return Clients.Client(receiverId).SendAsync("ReceiveOne", message);    // Note this 'ReceiveOne' 
        //}
        //public void saveConnectionId() { 
        //}

    }
}