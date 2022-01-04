using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class MessageRequest { 
    [Required]
    public int ReceiverId { get; set; }
    [Required]
    public string MessageContent { get; set; }
}
}
