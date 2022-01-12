using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string signalRconnectionId { get; set; }
    }
}