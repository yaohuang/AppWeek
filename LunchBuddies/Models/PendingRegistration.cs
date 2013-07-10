using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LunchBuddies.Models
{
    public class PendingRegistration
    {
        public string  Email { get; set; }
        public Guid Id { get; set; }
    }
}