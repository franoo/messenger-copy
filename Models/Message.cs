using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public int ReceiverId { get; set; }
        [Required]
        public string MessageContent { get; set; }
        [Required]
        public DateTime Date { get; set; }
       // [Required]
        //public int ConversationId{get;set;}
       // public int ConversationId { get; set; }
    }
}
