using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Demo_RealTime.Startup))]
namespace Demo_RealTime
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
