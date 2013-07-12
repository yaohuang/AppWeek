using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class UserViewModel
    {
        [Key]
        public string Email { get; set; }

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
    }
}
