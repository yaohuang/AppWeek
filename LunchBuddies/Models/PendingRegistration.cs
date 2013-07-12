using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LunchBuddies.Models
{
    public class PendingRegistration
    {
        public virtual UserViewModel User { get; set; }
        public Guid Id { get; set; }
    }
}