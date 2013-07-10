using System.Data.Entity.Config;
using System.Linq;
using LunchBuddies.Models;
using Owin;

namespace LunchBuddies
{
    public partial class Startup
    {
        public void ConfigureEntityFramework(IAppBuilder app)
        {
            using (var db = new ModelsDbContext())
            {
                var query = from b in db.Users
                            orderby b.Name
                            select b;
            }
        }
    }
}