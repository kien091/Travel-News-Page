using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_NET.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public int Status { get; set; }

        public ICollection<Articles> Articles { get; set; }
    }
}
