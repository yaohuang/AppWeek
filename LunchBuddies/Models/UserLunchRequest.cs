using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunchBuddies.Models
{
    public class UserLunchRequest
    {
        // complex key {LunchRequestId, UserId}
        public int LunchRequestId { get; set; }
        
        public string UserEmail { get; set; }

        [Required]
        public LunchStatus LunchStatus { get; set; }
    }
}
