using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class UserPictures
    {
        public long Id { get; set; }

        [Required]
        public User User { get; set; }

        public byte[] Picture { get; set; }

        public string Message { get; set; }
    }
}
