using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    class UserPictures
    {
        public long Id { get; set; }
        [Required]
        public User User { get; set; }
        public byte[] Picture { get; set; }
        public string Message { get; set; }
    }
}
