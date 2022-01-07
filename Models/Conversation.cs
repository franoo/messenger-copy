using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }
        [Required]
        public int ReceiverId { get; set; }
        [Required]
        public string MessageContent { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
