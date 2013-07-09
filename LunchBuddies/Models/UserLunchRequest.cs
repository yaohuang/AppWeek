using System;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    class UserLunchRequest
    {
        [Required]
        public LunchRequest LunchRequest { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public LunchStatus LunchStatus { get; set; }
    }
}
