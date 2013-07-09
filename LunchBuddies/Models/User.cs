using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class User
    {
        [Key]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Building { get; set; }
        public string Office { get; set; }

    }
}
