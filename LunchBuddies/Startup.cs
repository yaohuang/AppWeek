using Owin;

namespace LunchBuddies
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureEntityFramework(app);
            ConfigureAuth(app);

            app.MapHubs();
        }
    }
}
