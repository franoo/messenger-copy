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
        public List<Message> Messages { get; set; }
    }
}
