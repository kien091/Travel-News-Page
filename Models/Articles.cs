using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Final_NET.Models
{
    public class Articles
    {
        [Key]
        public int Id { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Content { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public int View { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }
    }
}
