using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class User : IUser
    {
        public User()
        {
            LunchRequests = new List<UserLunchRequest>();
        }

        public string Id { get; set; }

        [Key]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Office { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string Alias { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Title { get; set; }

        public string PictureUrl { get; set; }

        public virtual ICollection<UserLunchRequest> LunchRequests { get; set; }

        public virtual ICollection<LunchRequest> CreatedRequests { get; set; }
    }
}
