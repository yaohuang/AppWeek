using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class UserLunchRequest
    {
        [Required]
        public LunchRequest LunchRequest { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public LunchStatus LunchStatus { get; set; }
    }
}
