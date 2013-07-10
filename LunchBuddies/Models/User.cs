using System.Collections.Generic;
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

        [Required]
        public string Name { get; set; }

        [Required]
        public string Building { get; set; }

        [Required]
        public string Office { get; set; }

        public ICollection<UserLunchRequest> LunchRequests { get; set; }
    }
}
