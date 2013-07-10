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
        public DateTime BeginTime { get; private set; }
        [Required]
        public DateTime EndTime { get; private set; }

        public void setStartTime(DateTime startTime, DateTime endTime)
        {
            //var startTimeOfDay = startTime.TimeOfDay;
            //var endTimeOfDay = endTime.TimeOfDay;
            var comparison = startTime.CompareTo(endTime);

            if (comparison < 0)
            {
                BeginTime = startTime;
                EndTime = endTime;
            }
            else
            {
                throw new ArgumentException("fix yr times, make startTime earlier than endTime");
            }
        }
    }
}
