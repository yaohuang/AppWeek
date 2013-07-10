using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Config;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace LunchBuddies.Models
{
    public class ModelsDbConfiguration : DbConfiguration
    {
        public ModelsDbConfiguration()
        {
            LocalDbConnectionFactory factory = new LocalDbConnectionFactory("v11.0", @"AttachDbFilename=|DataDirectory|\LunchBuddies.mdf;Initial Catalog=LunchBuddies;Integrated Security=True");
            SetDefaultConnectionFactory(factory);
        }
    }
}