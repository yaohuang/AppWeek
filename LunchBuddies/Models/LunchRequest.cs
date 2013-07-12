using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class LunchRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MeetingPlace { get; set; }

        [Required]
        public virtual User Creator { get; set; }

        public DateTime DateTimeCreated { get; set; }

        public DateTime DateTimeRequest { get; set; }

        public virtual ICollection<UserLunchRequest> Users { get; set; }

        public virtual ICollection<Interest> Interests { get; set; }

        public string Subject { get; set; }
    }
}
