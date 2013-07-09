using System;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class UserLunchTimes
    {
        public long Id { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        [Required]
        public DateTime BeginTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }
}
