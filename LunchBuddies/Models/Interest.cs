using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LunchBuddies.Models
{
    public class Interest
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<LunchRequest> LunchRequests { get; set; }
    }
}
