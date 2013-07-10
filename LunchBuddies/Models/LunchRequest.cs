using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class LunchRequest
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string MeetingPlace { get; set; }

        [Required]
        public User Creator { get; set; }

        public DateTime DateTimeCreated { get; set; }

        public DateTime DateTimeRequest { get; set; }

        public ICollection<UserLunchRequest> Users { get; set; }

        public ICollection<Interest> Interests { get; set; }

        public string Subject { get; set; }
    }
}
