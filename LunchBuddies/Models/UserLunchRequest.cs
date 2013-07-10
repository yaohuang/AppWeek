using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunchBuddies.Models
{
    public class UserLunchRequest
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public LunchRequest LunchRequest { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public LunchStatus LunchStatus { get; set; }
    }
}
