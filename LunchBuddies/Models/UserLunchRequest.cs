using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunchBuddies.Models
{
    public class UserLunchRequest
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public virtual LunchRequest LunchRequest { get; set; }

        public LunchStatus LunchStatus { get; set; }
    }
}
