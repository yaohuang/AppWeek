﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LunchBuddies.Models
{
    public class PendingRegistration
    {
        public virtual User User { get; set; }
        public virtual UserViewModel UserView { get; set; }
        public Guid Id { get; set; }
    }
}